using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace XyBorg.Security
{
    class SecuritySystem
    {
        private bool check_passed = false;
        public SecuritySystem(string _content_path)
        {
            content_path = _content_path;
        }
        string content_path = "";
        internal void CreateFile()
        {
#if true
            throw new Exception("Невозможно создать!");
#else
            StreamReader sys_file = new StreamReader(content_path + "sys");
            StreamWriter sys_file_encoded = new StreamWriter(content_path + "sys.enc");
            string data = sys_file.ReadToEnd();
            for (int i = 0; i < data.Length; i++ )
            {
                char next = data[i];
                sys_file_encoded.Write((char)(next ^ 136));
            }
            sys_file.Close();
            sys_file_encoded.Close();
#endif
        }

        internal bool fails()
        {
            StreamReader sys_file = new StreamReader(content_path + "sys");
            StreamReader sys_file_encoded = new StreamReader(content_path + "sys.enc");
            string data = sys_file.ReadToEnd();
            string data_encoded = sys_file_encoded.ReadToEnd();
            for (int i = 0; i < data.Length; i++ )
            {
                if ((data[i] ^ 136) != data_encoded[i])
                {
                    check_passed = false;
                    return false;
                }
            }
            check_passed = true;

            sys_file.Close();
            sys_file_encoded.Close();


            return !check_passed;
        }
    }
}
