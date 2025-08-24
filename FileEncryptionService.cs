using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_Organizer__Command_Line_Tool___5
{
    class FileEncryptionService
    {
        AesEncryptionAndDecryptionService _AesService;
        public FileEncryptionService(string inputFileName, string encryptedFileName, string decryptedFileName, string folderName)
        {
            _AesService = new AesEncryptionAndDecryptionService(inputFileName, encryptedFileName, decryptedFileName, folderName);
        }
        public void PerformAesEncryption()
        {
            _AesService.EncryptFile();
        }
        public void PerformAesDecryption()
        {
            _AesService.DecryptFile();
        }
    }
}
