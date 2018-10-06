using System.IO;
using System.Collections;

public class salt
{
    private const string SALT_NAME = "salt.bin";
    private const int SALT_LENGTH = 14;
    private string saltPath;

    // I will put salt in different path when in different device(ios , android ... )
    public salt()
    {
		saltPath = DeviceInfo.PersisitFullPath(SALT_NAME);
    }

    private bool isSaltExist()
    {
        return File.Exists(saltPath);
    }

    // --- create salt ----
    private byte[] generateSalt()
    {
        PseudoRandom mRandom = PseudoRandom.getInstance();

        byte[] content = mRandom.generateRandom(SALT_LENGTH);
        // Create the file and write to it.
        // DANGER: System.IO.File.Create will overwrite the file if it already exists. 
        using (FileStream fs = File.Create(saltPath))
        {
            fs.Write(content, 0, content.Length);
        }
        return content;
    }
    // ---  read salt from file -----
    private byte[] readSalt()
    {
        byte[] content = new byte[SALT_LENGTH];
        using (FileStream fs = File.Open(saltPath, FileMode.Open))
        {
            fs.Read(content, 0, SALT_LENGTH);
        }
        return content;
    }
    //  get salt from file or create salt
    public byte[] getSalt()
    {
        if (isSaltExist())
        {
            return readSalt();
        }
        else
        {
            return generateSalt();
        }
    }

}