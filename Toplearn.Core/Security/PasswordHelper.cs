using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TopLearn.Core.Security
{
    public static class PasswordHelper
    {
        public static string EncodePasswordMd5(this string pass) //Encrypt using MD5   
        {

	        //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)   
			MD5 md5 = new MD5CryptoServiceProvider();
            var originalBytes = Encoding.Default.GetBytes(pass);
            var encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string   
            return BitConverter.ToString(encodedBytes);
        }


    }
}
