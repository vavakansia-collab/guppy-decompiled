using System;
using System.Security.Cryptography;

namespace CommonLibrary;

public class RandomPassword
{
	private static int DEFAULT_MIN_PASSWORD_LENGTH = 8;

	private static int DEFAULT_MAX_PASSWORD_LENGTH = 20;

	private static string PASSWORD_CHARS_LCASE = "qwertyuiopasdfghjklzxcvbnm";

	private static string PASSWORD_CHARS_UCASE = "QWERTYUIOPASDFGHJKLZXCVBNM";

	private static string PASSWORD_CHARS_NUMERIC = "0123456789";

	public static string Generate()
	{
		return Generate(DEFAULT_MIN_PASSWORD_LENGTH, DEFAULT_MAX_PASSWORD_LENGTH);
	}

	public static string Generate(int length)
	{
		return Generate(length, length);
	}

	public static string Generate(int minLength, int maxLength)
	{
		if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
		{
			return null;
		}
		char[][] charGroups = new char[3][]
		{
			PASSWORD_CHARS_LCASE.ToCharArray(),
			PASSWORD_CHARS_UCASE.ToCharArray(),
			PASSWORD_CHARS_NUMERIC.ToCharArray()
		};
		int[] charsLeftInGroup = new int[charGroups.Length];
		for (int i = 0; i < charsLeftInGroup.Length; i++)
		{
			charsLeftInGroup[i] = charGroups[i].Length;
		}
		int[] leftGroupsOrder = new int[charGroups.Length];
		for (int j = 0; j < leftGroupsOrder.Length; j++)
		{
			leftGroupsOrder[j] = j;
		}
		byte[] randomBytes = new byte[4];
		new RNGCryptoServiceProvider().GetBytes(randomBytes);
		Random random = new Random(BitConverter.ToInt32(randomBytes, 0));
		char[] password = null;
		password = ((minLength >= maxLength) ? new char[minLength] : new char[random.Next(minLength, maxLength + 1)]);
		int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
		for (int k = 0; k < password.Length; k++)
		{
			int nextLeftGroupsOrderIdx = ((lastLeftGroupsOrderIdx != 0) ? random.Next(0, lastLeftGroupsOrderIdx) : 0);
			int nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];
			int lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;
			int nextCharIdx = ((lastCharIdx != 0) ? random.Next(0, lastCharIdx + 1) : 0);
			password[k] = charGroups[nextGroupIdx][nextCharIdx];
			if (lastCharIdx == 0)
			{
				charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
			}
			else
			{
				if (lastCharIdx != nextCharIdx)
				{
					char temp = charGroups[nextGroupIdx][lastCharIdx];
					charGroups[nextGroupIdx][lastCharIdx] = charGroups[nextGroupIdx][nextCharIdx];
					charGroups[nextGroupIdx][nextCharIdx] = temp;
				}
				charsLeftInGroup[nextGroupIdx]--;
			}
			if (lastLeftGroupsOrderIdx == 0)
			{
				lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
				continue;
			}
			if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
			{
				int temp2 = leftGroupsOrder[lastLeftGroupsOrderIdx];
				leftGroupsOrder[lastLeftGroupsOrderIdx] = leftGroupsOrder[nextLeftGroupsOrderIdx];
				leftGroupsOrder[nextLeftGroupsOrderIdx] = temp2;
			}
			lastLeftGroupsOrderIdx--;
		}
		return new string(password);
	}
}
