using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    private string encryptionKey = "SenhaCriptografada";

    public GameData gameData = new GameData();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadData();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.F))
        {
            SaveData();
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            LoadData();
        }

        if (Input.GetKeyUp(KeyCode.Delete))
        {
            DeleteSaveData();
        }
#endif
    }

    public void SaveData()
    {
        SaveGameData();
    }

    private void SaveGameData()
    {
        string json = JsonUtility.ToJson(gameData);
        string encryptedData = Encrypt(json, encryptionKey);
        PlayerPrefs.SetString("EncryptedGameData", encryptedData);
        PlayerPrefs.Save();
        Debug.Log("Saved game!");
    }

    public void LoadData()
    {
        LoadGameData();
    }

    private void LoadGameData()
    {
        if (PlayerPrefs.HasKey("EncryptedGameData"))
        {
            string encryptedData = PlayerPrefs.GetString("EncryptedGameData");
            string decryptedData = Decrypt(encryptedData, encryptionKey);
            gameData = JsonUtility.FromJson<GameData>(decryptedData);
            Debug.Log("Loaded game!");
        }
        else
        {
            Debug.LogWarning("No game saved data found.");
        }
    }

    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteKey("EncryptedGameData");

        PlayerPrefs.Save();
        Debug.Log("Save data deleted.");
        TransitionController.instance.TransitionToSceneName("LevelSelector");
        Destroy(gameObject);
    }

    private string Encrypt(string data, string key)
    {
        byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(data);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(key);

        passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

        byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

        string encryptedData = Convert.ToBase64String(bytesEncrypted);

        return encryptedData;
    }

    private string Decrypt(string data, string key)
    {
        byte[] bytesToBeDecrypted = Convert.FromBase64String(data);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(key);

        passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

        byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

        string decryptedData = Encoding.UTF8.GetString(bytesDecrypted);

        return decryptedData;
    }

    private byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
    {
        byte[] encryptedBytes = null;

        using (var ms = new MemoryStream())
        {
            using (var AES = new RijndaelManaged())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;
                AES.Key = passwordBytes;
                AES.Mode = CipherMode.CBC;

                AES.GenerateIV();
                ms.Write(AES.IV, 0, 16);

                using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                    cs.Close();
                }
                encryptedBytes = ms.ToArray();
            }
        }
        return encryptedBytes;
    }

    private byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
    {
        byte[] decryptedBytes = null;

        using (var ms = new MemoryStream())
        {
            using (var AES = new RijndaelManaged())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;
                AES.Key = passwordBytes;
                AES.Mode = CipherMode.CBC;

                var iv = new byte[16];
                Array.Copy(bytesToBeDecrypted, 0, iv, 0, 16);
                AES.IV = iv;

                using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeDecrypted, 16, bytesToBeDecrypted.Length - 16);
                    cs.Close();
                }
                decryptedBytes = ms.ToArray();
            }
        }
        return decryptedBytes;
    }
}
