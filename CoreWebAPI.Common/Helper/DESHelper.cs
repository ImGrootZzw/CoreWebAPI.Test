using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AutoLoginNetterm.Helper
{
    public class DESHelper
    {
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="key">密钥，必须8位</param>
        /// <param name="iv">向量，必须8位</param>
        /// <param name="data">明文</param>
        /// <returns></returns>
        public static string DESEncrypt(string key, string iv, string data)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(key);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(iv);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="key">密钥，必须8位</param>
        /// <param name="iv">向量，必须8位</param>
        /// <param name="data">密文</param>
        /// <returns></returns>
        public static string DESDecrypt(string key, string iv, string data)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(key);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(iv);

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return null;
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }

        ///// <summary>
        ///// DES加密方法
        ///// </summary>
        ///// <param name="strPlain">明文</param>
        ///// <param name="strDESKey">密钥</param>
        ///// <param name="strDESIV">向量</param>
        ///// <returns>密文</returns>
        //public string DESEncrypt(string strPlain, string strDESKey, string strDESIV)
        //{
        //    //把密钥转换成字节数组
        //    byte[] bytesDESKey = ASCIIEncoding.ASCII.GetBytes(strDESKey);
        //    //把向量转换成字节数组
        //    byte[] bytesDESIV = ASCIIEncoding.ASCII.GetBytes(strDESIV);
        //    //声明1个新的DES对象
        //    DESCryptoServiceProvider desEncrypt = new DESCryptoServiceProvider();
        //    //开辟一块内存流
        //    MemoryStream msEncrypt = new MemoryStream();
        //    //把内存流对象包装成加密流对象
        //    CryptoStream csEncrypt = new CryptoStream(msEncrypt, desEncrypt.CreateEncryptor(bytesDESKey, bytesDESIV), CryptoStreamMode.Write);
        //    //把加密流对象包装成写入流对象
        //    StreamWriter swEncrypt = new StreamWriter(csEncrypt);
        //    //写入流对象写入明文
        //    swEncrypt.WriteLine(strPlain);
        //    //写入流关闭
        //    swEncrypt.Close();
        //    //加密流关闭
        //    csEncrypt.Close();
        //    //把内存流转换成字节数组，内存流现在已经是密文了
        //    byte[] bytesCipher = msEncrypt.ToArray();
        //    //内存流关闭
        //    msEncrypt.Close();
        //    //把密文字节数组转换为字符串，并返回
        //    return UnicodeEncoding.Unicode.GetString(bytesCipher);
        //}

        ///// <summary>
        ///// DES解密方法
        ///// </summary>
        ///// <param name="strCipher">密文</param>
        ///// <param name="strDESKey">密钥</param>
        ///// <param name="strDESIV">向量</param>
        ///// <returns>明文</returns>
        //public string DESDecrypt(string strCipher, string strDESKey, string strDESIV)
        //{
        //    //把密钥转换成字节数组
        //    byte[] bytesDESKey = ASCIIEncoding.ASCII.GetBytes(strDESKey);
        //    //把向量转换成字节数组
        //    byte[] bytesDESIV = ASCIIEncoding.ASCII.GetBytes(strDESIV);
        //    //把密文转换成字节数组
        //    byte[] bytesCipher = UnicodeEncoding.Unicode.GetBytes(strCipher);
        //    //声明1个新的DES对象
        //    DESCryptoServiceProvider desDecrypt = new DESCryptoServiceProvider();
        //    //开辟一块内存流，并存放密文字节数组
        //    MemoryStream msDecrypt = new MemoryStream(bytesCipher);
        //    //把内存流对象包装成解密流对象
        //    CryptoStream csDecrypt = new CryptoStream(msDecrypt, desDecrypt.CreateDecryptor(bytesDESKey, bytesDESIV), CryptoStreamMode.Read);
        //    //把解密流对象包装成读出流对象
        //    StreamReader srDecrypt = new StreamReader(csDecrypt);
        //    //明文=读出流的读出内容
        //    string strPlainText = srDecrypt.ReadLine();
        //    //读出流关闭
        //    srDecrypt.Close();
        //    //解密流关闭
        //    csDecrypt.Close();
        //    //内存流关闭
        //    msDecrypt.Close();
        //    //返回明文
        //    return strPlainText;
        //}
    }
}
