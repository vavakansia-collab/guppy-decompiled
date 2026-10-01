# Guppy.bet (AccountsManager) — полная реконструкция архитектуры

> Документ составлен по результатам декомпиляции клиентских сборок (ILSpy 9.1).
> Декомпилированные исходники: `C:\Users\Andrey\Desktop\guppy-decompiled\src\` (334 файла).
> Охват: клиент — по коду; сервер — по контрактам (протокол, DTO, обработка ответов).
> Документ аналитический; серверный код guppy в распоряжении не попадал и не реконструировался дословно.

---

## 1. Что это за продукт

Десктопное WPF-приложение (.NET Framework 4.8) для Windows — **исполнительный пульт сервиса guppy.bet**: управление аккаунтами букмекеров и телеметрия ставок.
Назначение: держать десятки аккаунтов букмекеров (легальный российский сегмент + офшорные),
автоматически логиниться, поддерживать живые сессии, передавать их серверу и отображать телеметрию ставок.
Парсинг линий, сравнение матчей, расчёт стратегий и исполнение ставок живут на сервере компании.

**Поддерживаемые букмекеры (20 парсеров-провайдеров, список из `ProvidersCommon.ProvidersNames`):**

`_1Win`, `_1xbet`, `Marathon`, `MarathonRU`, `BaltbetRU`, `BetBoom`, `Betcity`, `BetcityLine`,
`BeterCo`, `Parimatch`, `Tennisi`, `FonbetRU`, `FonbetRULine`, `LigaStavokRU`, `OlimpBet`,
`SetkaCupProvider`, `Winline`, `WinlineLine`, `LeonRU`, `ZenitWin`

Особенности списка: live-линия и прематч у Betcity/Fonbet/Winline — отдельные парсеры (`*Line`);
выделенный парсер Setka Cup (турниры по настольному теннису). Для исполнения (автологин) дополнительно
поддержаны: `_1xbit`, `_1xstavka`, `Melbet`, `MelbetRU`, `Betwinner`, `Leon`, `ParibetRU`, `BetteryRU`, `OlimpCom`.

---

## 2. Общая архитектура (микросервисы на сервере)

Перечисление `CommonLibrary.TypesProject` даёт полный состав их платформы:

| Сервис | Роль |
|---|---|
| **Provider** | парсеры линии букмекеров (20 шт.) |
| **DataCenter** | хранилище линий/снапшотов, приём дельт |
| **Scout** | сопоставление матчей между букмекерами |
| **CompareCenter** | «compare matches» — склейка матчей между букмекерами |
| **StrategiesCenter** | расчёт стратегий, генерация сигналов |
| **BettingCenter** | исполнение ставок через API с токенами аккаунтов |
| **AccountsCenter** | база аккаунтов/владельцев/прокси, аутентификация операторов |
| **UserCompareMatches / AccountsManager** | клиентские роли |

**Транспорт:** WatsonTcp (TCP, без TLS), порты 9010–9011, центральный сервер `185.216.192.14` (`CommonStaticFunction.GetIpServer()`).
Сообщения — JSON `AppExchange { TypeProject, TypeData, DataStr }` (Newtonsoft, местами `TypeNameHandling.All` — дыра)
либо protobuf (`ProtobufAM`/`ProtobufBC`, RuntimeTypeModel). Ping каждые 5 с, реконнект при тишине >30 с.
Внутренние машины: `STELLARBOUND` (dev), `SERVER2696X2` (main), пути данных `d:\guppy.bet` / `E:\guppy.bet`.

Словарь сообщений — `CommonLibrary.TypesData`: 50+ типов (GetMatches*, ProviderMatches*, Auth, GetAccounts,
UpdateProxy, ExecuteCommand, AccountCookie, GetSnapshotsByData, ProviderMatchesDelta и т.д.).

---

## 3. Каноническая модель данных (во что нормализуются все парсеры)

### 3.1 Каталог спорта — `SportBL` / `Sport` (17 видов)

Каждый вид спорта несёт **поведенческие флаги**, управляющие всей дальнейшей логикой:

| Поле | Смысл |
|---|---|
| `CompareTotalScore` | сравнивать ли общий счёт при матчинге |
| `IsConsistentScore` | счёт монотонный (не убывает) |
| `IsSlowTimeGoal` | «медленные голы» — влияет на retention истории очков |
| `IsThreeWayType` | 3 исхода (П1/Х/П2) или 2 |
| `IsCancelPoints` | очки могут отменяться (теннис!) |
| `IsFullScoreParallelPeriod` | полный счёт = сумма параллельных периодов |
| `PeriodDurationMinutesDefault` | тайм/период/четверть/сет/партия (футбол 45, хоккей 20, баскетбол 10…) |
| `TotalPointsToCompare` | сколько последних очков периода хранить (волейбол 10, н/теннис 12) |

Примеры: Футбол(id 1, 3-way, отмена очков, 45 мин), Хоккей(2), Баскетбол(3), Теннис(4, 2-way, отмена очков),
Волейбол(6, 2-way), Настольный теннис(10), Бадминтон(16), Пляжный волейбол(29) и др.

Типы матчей (`TypesMatch`): `Regular` / `Cyber` / `Short` — у спорта флаги доступности по каждому типу.

### 3.2 Каталог маркетов — `IssueType` (38 значений)

Тоталы ТБ/ТМ, ИТБ/ИТМ 1/2, Фора 1/2, П1/Х/П2, 1X/12/X2, Чёт/Нечёт, Очко П1/П2, Гонка П1/П2,
теннисные геймы (Гейм П1/П2, точные счета гейма 40:0/40:15/40:30/40:40, «15:15 да/нет», «30:30 да/нет», «40:40 да/нет»).

**Идентичность купона (исхода) = `(IssueType, IssueValue, Period, GameTennis)`.**
Есть операция `Overturn()` — отображение исхода в противоположный (для склейки букмекеров с обратной записи форы).

### 3.3 Матч — `Match` / `MatchLine`

`NameProvider, IdMatch (id у букмекера), IdChamp, NameChamp, IdSport, Team1/2 (+First/LastName для киберспорта),
StartTime/FinishTime, Period, ElapsedTime, TypeMatch, Url, Parameters(словарь), Events, Coupons`.

### 3.4 Счёт — `Score` / `PointScore`

Двойное представление:
- строка `StrScore` вида `"2:1(1:0,1:1)"` (теннис + subScore в скобках);
- история `List<PointScore>` — по очку: `Period, ValuePointTeam1/2, TimeCreate, TimeServerLoad, NumberGetTeam, IsDefault`.

Нормализация строки (сеттер `StrScore`):
```
patternScore         = ^\d*:\d*  \((.*?)\)
patternScoreTennis   = ^\d*:\d*  \((.*?)\)  \(([^)]+)\)[^)]*$   // + subScore
```
- `"0:0"` → `"0:0(0:0)"`; замены `ADV/Adv/AD/50 → A`, `00 → 0` (в subScore/периодах);
- **валидация**: число групп периодов должно быть ровно `число ':' в полном счёте − 1`, иначе счёт
  отбрасывается событием `OnErrorUpdateScoreHandler` («Не смогли обновить счёт») — кривые данные не ходят дальше;
- `Server` (подача, 0/1/2), `IsScoreUpdated` — детект фактического изменения.
- Retention: для спортов без `IsSlowTimeGoal` на период хранятся только последние `TotalPointsToCompare + 2` очка.

### 3.5 Купон и линия

- `Coupon`: `Availability, Period, GameTennis, IssueType, IssueValue, PayoutRatio, LastTimeUpdate, Parameters`.
- `CouponLine : Coupon` + `List<CoefFrame> HistoryCoefs` — **история коэффициентов** (`CoefFrame { Timestamp, PayoutRatio }`).
- `MatchLine : Match`-аналог с `List<CouponLine> CouponsLine` — линия с историей (для снапшотов/DropCoef).

---

## 4. Как работают парсеры (реконструкция по контракту)

Схема конвейера каждого Provider-сервиса:

```
poll источника (~IntervalUpdate = 5000 мс)
  → полный слепок Provider { Matches: List<Match> } (канонические объекты)
  → WatsonTcp на DataCenter (порт 9011):
      TypesData.ProviderMatches        — полный слепок
      TypesData.ProviderMatchesLine    — линия с историей коэф.
      TypesData.ProviderMatchesDelta   — дельта
  → SyncResponse "OK" / текст ошибки; ping 5 c; реконнект >30 c тишины
```

Формат дельты — `MatchLineDto`:
- `AddMatches: List<MatchLine>` — новые/изменённые матчи целиком;
- `ChangesCouponFrame: List<(idMatch, CouponLineDto, List<CoefFrame>)>` — только движения коэффициентов с фреймами.

### 4.1 Мерж линии на принимающей стороне (`ProvidersCommon.UpdateDataProvider` + `UpdateCoupons`)

Оброверженные боем правила (полезны как чек-лист любому движку мониторинга линий):

1. Матч ищется по `IdMatch`; новый — добавляется.
2. **Смена команд при том же IdMatch → матч «пересобран» (id переиспользован): купоны сбрасываются, матч удаляется.**
3. Счёт обновляется только при реальном изменении (`DecomposedScore`-сравнение); новые PointScore докладываются по GUID (дедуп).
4. Матч, исчезнувший из слепка: помечается `FinishTime`, **удаляется только через 10 минут** без обновлений (антифликер).
5. Купоны: обновление по ключу `(IssueType, IssueValue, Period, GameTennis)`; изменение коэф./availability
   обновляет `LastTimeUpdate`; купоны, отсутствующие в новом слепке, удаляются (сразу — в отличие от матчей).
6. **Встроенный детектор инсайдерского движения** (`OnStrategy1`, срабатывает в момент мержа):
   волейбол/настольный теннис (idSport 6/10), исход П1/П2 **будущего** периода, текущий коэф 1.01–2.5,
   скачок ≥0.5 за <60 c после обновления, не Marathon, не «CTT 21», период ещё не завершён → сигнал.

---

## 5. Стратегии

Перечисление `TypesStrategy`: `Poslegoal, Value, ValuePlus, ValueScore`. Типы ставок (`TypeBet`):
`Random, Poslegol, Value, ValuePlus, ValueScore`. Расчёт результата — `TypesCalculationBet`:
`NotCalculation, Unclear, Win, Lose`.

Все стратегии в клиенте — DTO с сервера (`StrategyLibrary`); параметрика (`*Settings` в AccountsLibrary):

| Стратегия | Ключевые параметры |
|---|---|
| Value | `MinPayoutRatioCompare`, `MaxPayoutRatioCompare`, `MinPercentage`, `HalfPeriod*` |
| ValuePlus | `HalfPeriod*`, `OnlyValueEmpty` |
| ValueScore | `HalfPeriod*` |
| Poslegol («после гола») | `IsBetMuchTimeScore` |
| Random (маскировка) | `Percent`, `MinSumBet/MaxSumBet`, `MinPayoutRatio/MaxPayoutRatio`, `SendBetTelegram` |

В DTO стратегий: пары матчей у двух провайдеров, `IsOverturn` (противоположный исход), `DifferencePoints`,
`DifferencePointsTeams`, `IsTwoHalfPeriod`, `TimeLastScore`, `NumberSDP` (номер сканерного сигнала).

---

## 6. Исполнение: вход, анти-детект, ставка

### 6.1 Браузерная ферма (`Browser.cs`, 1587 строк)

- Свой Chrome: `ChromeDriver\Chrome\chrome.exe` + драйвер; **DevTools-сессия v113**.
- На аккаунт: свой прокси (расширение + локальный **Titanium.Web.Proxy**), свой UserAgent
  (пул из 742 мобильных + десктопных UA в `CommonStaticFunction`), изолированный кэш
  `WebCache\_Chrome\<букмекер>\<guid>\cookie_backup.txt` (сохранение/загрузка cookies Selenium).
- Циклы-воркеры: `LoopAutoPaste` (автозаполнение), `LoopResponse` (перехват XHR/Fetch ответов),
  `LoopTabs`, `LoopName`.

**Анти-детект (3 уровня):**
1. `navigator.webdriver → undefined` (инжект при старте);
2. непрерывный (каждую ~1 мс) инжект JS-подмены фингерпринта: WebGL vendor/GPU (рендом из списка,
   Google (AMD)/(Nvidia)), `hardwareConcurrency: 8`, screen 1600×900, canvas-toDataURL (заглушка),
   **обнуление `cdc_adoQpoasnfa76pfcZLmcfl_*` переменных** (следы ChromeDriver);
3. программный обход **Qrator** (см. 6.3).

### 6.2 Автологин (`LoopAutoPaste`)

Для каждого букмекера — ветка с захардкоженными XPath (`/html/body/div[2]/div[1]/.../form/div[1]/div/input`):
проверка пустоты поля → `SendKeys(логин)` → sleep → пароль → чекбокс. Учёт форматов логина
(`+7...` vs email: переключение таба телефона/почты — 1xbet, Marathon, Betwinner).

### 6.3 Обход Qrator (LigaStavok) — `QratorCookies.cs`, полный алгоритм

1. GET `https://<host>/` → cookie вида `<nonce>-<qsessid>`.
2. GET `/__qrator/qauth_utm_v2c_v41b5.js` (челлендж-скрипт).
3. **Proof-of-work**: брутфорс счётчика n, пока `MD5(nonce + n)` не начнётся с `00` → `pow = n`.
4. В бинарник зашит **зашифрованный шаблон фингерпринта** (~40 полей: `ua, br, perf, n, ws, ws2, ws3, bv, st, wl, wc,
   wci, wci2, wci3, wx0, wx1, cpt, ni, nf, mev, dhf, mhs, cssb, sct, ft, nb, md, lsss, ssv, wvps, t, tc, c, pvc, pvm,
   cc, df, st2-4, version, vx`), закодированный как `v1$<salt>$<base64>`.
   - Расшифровка: XOR с ключом `MD5(salt + nonce + pow)` (base64 → latin1 → XOR → JSON).
   - В шаблон подставляются: настоящий UserAgent (из Selenium `navigator.userAgent`) и свежие тайминги
     `PerformanceTiming` (все поля navigation/dom/load → текущий timestamp).
   - Обратно шифруется тем же ключом со случайным salt.
5. POST `/__qrator/validate?pow=<pow>&nonce=<nonce>&qsessid=<qsessid>` с телом-фингерпринтом,
   полным набором `sec-ch-ua*` заголовков, через сырой TCP (`ExtendedTcpClient`, CONNECT через прокси).
6. Ответ даёт 3 cookie (`jsid` и др.) → инжект в Chrome → refresh. Итог: сессия без браузера.

### 6.4 Сборка сессий и исполнение ставок

- **DevTools-перехват ответов** (`GetResponseBodyAsync`) вытаскивает токены авторизации в параметры аккаунта:
  - BetBoom: `x_access_token`, `RefreshToken` ← `/api/auth/login`;
  - Parimatch: `token` ← `/RoutingLogin`, `status` ← `/user/getAccountInfo`;
  - Betcity: `dev`-параметр из URL `/auth?`, `block_status` из тела;
  - LigaStavok: `AccountNumber` ← `/getBalance`, `vipClient` ← `/getUserInfo`.
- По запросу сервера (`TypesData.AccountCookie`, sync-request) клиент отдаёт **куки и токены аккаунта**
  (`cookie_backup.txt`) на BettingCenter.
- **Ставки ставит сервер через API букмекера с этими токенами.** Клиент видит только телеметрию:
  `Bet { Sum, Profit = floor(k×Sum), MaxSum, MaxBetResponce, TimeResponce, BalanceBefore, Success,
  Status: Create→InProcess→Complite/Cancel, TypeChangePayoutRatio, TypesLine }`,
  обёртка `BetStrategy { AccountSettings, Bet, CustomStrategy, StrategySettings, NumberSDP }`.
- Пульт (`AccountCommandEnum`): Start/Stop, GetBalance, ClearMessages, сброс статусов стратегий,
  **PutRandomBet** (случайная ставка — маскировка аккаунта «живым» поведением; настройки сумм/коэф. в RandomSettings).

Итого: **браузер — фабрика сессий (и проход капч/верификаций руками оператора), сервер — исполнитель ставок.**

---

## 7. Фарминг аккаунтов букмекеров (CRM)

Модель `AccountsLibrary`: `Player` → `Passport` (ФИО, серия, номер, даты, код подразделения), `SimCard`
(оператор, страна, владелец), `CardBank` (банк из `EnumBanks`: Сбер/Альфа/ВТБ/ПСБ/Райфайзен/Почта/РостФинанс/Тинькофф;
номер ≤16 цифр), `MailBox` (почта+пароль), `PaymentSystem`, `Photo/PhotoType` + `folderOfFotos` (верификация),
`RegOperator` (сервис регистрации), `StatusesRegistration`, `OwnerAccount` (владелец-оператор).
Статусы аккаунта: `StatusAccount/StatusWorking/StatusesRegistration/StatusesBet`; флаги `UniqueBets`,
`TakeSecondHand` (ставки «с рук»), `Prioritet`, `CheckProxyOnStart`, `IsClearCookieBrowser`, `EnableAutoFillingBrowser`.

## 8. Прокси

`Proxy` (GUID, логин/пароль, `webProxy`), централизованная выдача с сервера
(`GetAvabilityProxyAccountSyncRequestAsync(bookmaker)` — сервер подбирает свободный прокси под букмекера),
проверка IP: `api.ipify.org`, `2ip.ru`. Окно управления `WindowProxy`, учёт срока (`ConvertProxyDateRemain`).

## 9. Снапшоты и DropCoef

- `SnapshotsLibrary`: периодический полный слепок линии (счёт с историей + купоны) по всем букмекерам;
  серверное слияние `SnapshotBL.MergeSnapshots` (по имени букмекера → id матча → ключу купона, склейка
  `ScoreHistory` и `CoefFrame`-историй). Отдача по датам (`GetSnapshotsByData`).
- **DropCoef** (модуль клиента): аналитика падений коэффициентов поверх линии с историей
  (`WatsonDataServerLineClient.GetMatchesCurrent/AtData/...`), фильтры по провайдеру/спорту/интервалу,
  статистика `MatchLineStat`/`CouponStat`.

## 10. Обновления и дистрибуция

`FtpUpdateClient`: FTP `31.31.196.215:21`, пользователь `u0377646_guppy` (пароль вшит в код),
проверка `update.zip` по mtime против локального `timestamp`; `AccountsManagerUpdater.exe`/`UpdateUploader.exe`.
Контакты: `t.me/guppy_bet`, `t.me/alexSmit_777`. Новости: RSS sport-express.ru в тайтле окна.

---

## 11. Выявленные слабости (у guppy)

1. **FTP-креды в открытом виде** + обновление по незашифрованному FTP (перехват/подмена апдейта).
2. **WatsonTcp без TLS**; Newtonsoft `TypeNameHandling.All` → потенциальная десериализационная RCE на сервере.
3. Куки/токены аккаунтов ходят клиент↔сервер в открытом виде; пароль оператора хранится локально (`LastPassword`).
4. Вся база аккаунтов (логины/пароли/паспорта/карты) централизованно на их сервере.
5. XPath-автологин ломается любым редизайном; `Thread.Sleep`-синхронизация повсюду.
6. Служебные имена машин и пути зашиты в продовый бинарь.

---

## 12. Выводы/уроки для собственного движка (BetRadar)

1. Конвейер guppy = наш конвейер (parsers → normalize → compare → engine); копировать нечего — серверного кода нет,
   но контракт обкатан в боях. Чек-лист для аудита своих парсеров:
   - идентичность исхода по ключу (маркет, значение, период, доп-параметр) — апдейт, а не пересоздание;
   - антифликер (задержка удаления пропавших сущностей; у них матчи 10 мин);
   - переиспользование id матча со сменой команд = сброс купонов;
   - строгая валидация счёта (regex + сверка периодов) до движка;
   - дельта-протокол с историей коэффициентов (`CoefFrame`) вместо полных слепков наверх.
2. Поведенческие флаги спорта (`IsCancelPoints`, `IsSlowTimeGoal`, `TotalPointsToCompare`) — дешёвый способ
   управлять retention истории очков и корректностью послеголов.
3. Дешёвый детект аномальных движений коэффициентов прямо в потоке обновлений (см. 4.1.6) —
   применим в собственном движке как скоринг сигналов.
4. Рецепт обхода Qrator (PoW + XOR-фингерпринт) — задокументирован в 6.3 (использование против живых сайтов —
   правовая серая зона).
5. BetBoom/Parimatch/Betcity отдают токены сессии стандартными эндпоинтами — сессию можно собрать и без браузера.
6. Подтверждение: легальные РФ-букмекеры API для ставок не дают — guppy вынужден держать браузерную ферму
   (Selenium + анти-детект + операторы на капчах). Это дорого и хрупко; наш подход (чистые парсеры + движок)
   ту же аналитическую ценность даёт без этого слоя.

---

## Приложение: карта декомпилированных исходников

```
src/
├── CommonLibrary/      — транспорт (Watson), AppExchange, Net (HTTP+proxy), Proxy, UA-пулы, Compress/Deflate
├── BookmakerLibrary/   — модель линии: Sport/SportBL, IssueType, Match, Score, Coupon(CoefFrame),
│                         ProvidersCommon (мерж), MatchLineDto, WatsonLineProviderClient, Provider
├── CompareLibrary/     — склейка матчей: CompareMatch (InnerId, IsOverturn), CompareMatchParameter
├── StrategyLibrary/    — DTO стратегий: Value, ValuePlus, ValueScore, Poslegol
├── AccountsLibrary/    — аккаунты/фарминг: AccountSettings, Player/Passport/SimCard/CardBank,
│                         ProtobufAM, настройки стратегий, AccountCommandEnum
└── AccountsManager/    — клиент: Browser.cs (ферма+автологин+DevTools+анти-детект), QratorCookies.cs,
                          AccountsCenterClient/BettingCenterClient, ViewModelMainWindow,
                          DropCoef/*, snapshotsviewer, FtpUpdateClient
```
