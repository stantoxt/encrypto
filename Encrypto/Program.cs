﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;
using Encrypto.AESLibrary;

namespace Encrypto
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<EncryptOptions, DecryptOptions>(args);

            result.MapResult(
                (EncryptOptions options) =>
                {
                    if (Directory.Exists(options.InputFile)) // Directory
                    {
                        Console.WriteLine("Encryption Started...");
                        string[] filesAndSubDirectories = Directory.GetFileSystemEntries(options.InputFile);
                        Console.WriteLine("Password:");
                        string pass = Console.ReadLine();
                        EncryptSubFolders(filesAndSubDirectories, pass);
                        Console.WriteLine("Encryption Finished...");
                    }
                    else if (File.Exists(options.InputFile)) //File Only
                    {
                        Console.WriteLine("Encryption Started...");
                        Console.WriteLine("Password:");
                        string pass = Console.ReadLine();
                        Console.WriteLine(string.Format("{0} | Result:{1}", options.InputFile,
                            AES.EncryptFile(options.InputFile, options.InputFile, pass)));
                        Console.WriteLine("Encryption Finished...");
                    }
                    else if (!string.IsNullOrEmpty(options.InputText))
                    {
                        Console.WriteLine("Encryption Started...");
                        Console.WriteLine("Password:");
                        string pass = Console.ReadLine();
                        Console.WriteLine(string.Format("{0}-->{1}{2}", options.InputText, Environment.NewLine, Base64Encode(AES.EncryptText(options.InputText, pass))));

                        Console.WriteLine("Encryption Finished...");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(options.InputFile) || string.IsNullOrEmpty(options.InputText))
                        {
                            Console.WriteLine("ERROR(S):");
                            Console.WriteLine("-i\tInput File(s) or Folder(s) to encrypt.");
                            Console.WriteLine("-t\tInsert the text to encrypt.");
                        }
                    }

                    return 0;
                },
                (DecryptOptions options) =>
                {
                    if (Directory.Exists(options.InputFile)) // Directory
                    {
                        Console.WriteLine("Decryption Started...");
                        Console.WriteLine("Password:");
                        string pass = Console.ReadLine();
                        string[] filesAndSubDirectories = Directory.GetFileSystemEntries(options.InputFile);
                        DecryptSubFolders(filesAndSubDirectories, pass);
                        Console.WriteLine("Decryption Finished...");
                    }
                    else if (File.Exists(options.InputFile)) //File Only
                    {
                        Console.WriteLine("Decryption Started...");
                        Console.WriteLine("Password:");
                        string pass = Console.ReadLine();
                        Console.WriteLine(string.Format("{0} | Result:{1}", options.InputFile,
                            AES.DecryptFile(options.InputFile, options.InputFile, pass)));
                        Console.WriteLine("Decryption Finished...");
                    }
                    else if (!string.IsNullOrEmpty(options.InputText))
                    {
                        Console.WriteLine("Decryption Started...");
                        Console.WriteLine("Password:");
                        string pass = Console.ReadLine();
                        byte[] base64decode = Base64Decode(options.InputText);
                        Console.WriteLine(string.Format("{0}-->{1}{2}", options.InputText, Environment.NewLine, AES.DecryptText(base64decode, pass)));
                        Console.WriteLine("Decryption Finished...");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(options.InputFile) || string.IsNullOrEmpty(options.InputText))
                        {
                            Console.WriteLine("-i\tInput File(s) or Folder(s) to decrypt.");
                            Console.WriteLine("-t\tInsert the text to decrypt.");
                        }
                    }

                    return 0;
                },
                errors =>
                {
                    var invalidTokens = errors.Where(x => x is TokenError).ToList();
                    if (invalidTokens != null)
                    {
                        invalidTokens.ForEach(token => Console.WriteLine(((TokenError)token).Token));
                    }

                    return 1;
                });
        }

        private static string ReadLines(string fileName, bool fromTop, int count)
        {
            var lines = File.ReadAllLines(fileName);
            if (fromTop)
            {
                return string.Join(Environment.NewLine, lines.Take(count));
            }

            return string.Join(Environment.NewLine, lines.Reverse().Take(count));
        }

        public static string Base64Encode(byte[] input)
        {
            return System.Convert.ToBase64String(input);
        }

        public static byte[] Base64Decode(string plainText)
        {
            return System.Convert.FromBase64String(plainText);
        }

        private static string ReadBytes(string fileName, bool fromTop, int count)
        {
            var bytes = File.ReadAllBytes(fileName);
            if (fromTop)
            {
                return System.Text.ASCIIEncoding.ASCII.GetString(bytes, 0, count);
            }

            return System.Text.ASCIIEncoding.Default.GetString(bytes, bytes.Length - count, count);
        }

        private static Tuple<string, string> MakeError()
        {
            return Tuple.Create("\0", "\0");
        }

        public static void EncryptSubFolders(string[] folder, string password)
        {
            foreach (string file in folder)
            {
                FileAttributes attr = File.GetAttributes(file);
                if (attr == FileAttributes.Directory)
                {
                    string[] filesAndSubDirectories = Directory.GetFileSystemEntries(file);
                    if (filesAndSubDirectories.Length > 0)
                        EncryptSubFolders(filesAndSubDirectories, password);
                }
                else
                {
                    Console.WriteLine(string.Format("{0} | Result:{1}", file, AES.EncryptFile(file, file, password)));
                }
            }
        }

        public static void DecryptSubFolders(string[] folder, string password)
        {
            foreach (string file in folder)
            {
                FileAttributes attr = File.GetAttributes(file);
                if (attr == FileAttributes.Directory)
                {
                    string[] filesAndSubDirectories = Directory.GetFileSystemEntries(file);
                    if (filesAndSubDirectories.Length > 0)
                        DecryptSubFolders(filesAndSubDirectories, password);
                }
                else
                {
                    Console.WriteLine(string.Format("{0} | Result:{1}", file, AES.DecryptFile(file, file, password)));
                }
            }
        }
    }
}
