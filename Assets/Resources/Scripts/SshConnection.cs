using UnityEngine;
using Renci.SshNet;

public class SshConnection : MonoBehaviour
{
    public TextMesh ConsoleText;
    private UnityEngine.UI.Text text = null;

    public string pushFile = null;
    public string pushContents = null;
    public string pullFile = null;
    public string pullContents = null;

    //PasswordConnectionInfo connectionInfo;

    void Start()
    {
        pushFile = null;
        pushContents = null;
        pullFile = null;
        pullContents = null;
        StartCoroutine(Connect());
        //text = this.GetComponent<UnityEngine.UI.Text>();
    }

    void Yeet()
    {

        try
        {
            var connectionInfo = new PasswordConnectionInfo(PrivateConstants.host, 22, PrivateConstants.ssh_user, PrivateConstants.ssh_pass);
            ConsoleText.text += "connection infos : ok\n";

            using (var client = new SshClient(connectionInfo))
            {
                ConsoleText.text += "Connecting...\n";
                client.Connect();
                ConsoleText.text += "OK\n";

                var command = client.RunCommand("pwd");
                ConsoleText.text += command.Result;

                ConsoleText.text += "Disconnecting...\n";
                client.Disconnect();
                ConsoleText.text += "OK\n";

                Debug.Log(text.text);
            }
        }
        catch (System.Exception e)
        {
            ConsoleText.text = "Error\n" + e;
            Debug.Log(text.text + e);

        }
    }

    public bool tryLogin()
    {
        return Input.GetKey(KeyCode.Alpha5) && Input.GetAxisRaw("Sprint") == 1;
    }

    public void writeToFile(string file, string str)
    {
        try
        {
            var connectionInfo = new PasswordConnectionInfo(PrivateConstants.host, 22, PrivateConstants.ssh_user, PrivateConstants.ssh_pass);
            ConsoleText.text += "connection infos : ok\n";

            using (var client = new SshClient(connectionInfo))
            {
                ConsoleText.text += "Connecting...\n";
                client.Connect();
                ConsoleText.text += "OK\n";

                ConsoleText.text += "Attempting to push to server";
                ConsoleText.text += client.RunCommand("echo \"" + str + "\" >> " + file).Result;

                ConsoleText.text += "Disconnecting...\n";
                client.Disconnect();
                ConsoleText.text += "OK\n";

                Debug.Log(ConsoleText.text);
            }
        }
        catch (System.Exception e)
        {
            ConsoleText.text = "Error\n" + e;
            Debug.Log(ConsoleText.text + e);
        }
    }

    public string readFromFile(string file)
    {
        string result = null;

        try
        {
            var connectionInfo = new PasswordConnectionInfo(PrivateConstants.host, 22, PrivateConstants.ssh_user, PrivateConstants.ssh_pass);
            ConsoleText.text = "connection infos : ok\n";

            using (var client = new SshClient(connectionInfo))
            {
                ConsoleText.text += "Connecting...\n";
                client.Connect();
                ConsoleText.text += "OK\n";

                result = client.RunCommand("cat " + file).Result;

                ConsoleText.text += "Disconnecting...\n";
                client.Disconnect();
                ConsoleText.text += "OK\n";

                Debug.Log(text.text);
            }
        }
        catch (System.Exception e)
        {
            ConsoleText.text = "Error\n" + e;
            Debug.Log(text.text + e);
        }

        return result;

    }

    public bool connection = false;
    bool reconnect = false;
    public TextMesh connectionText;
    int pingCounter = 0;
    System.Collections.IEnumerator Connect()
    {
        yield return new WaitForEndOfFrame();
        reconnect = false;
        //ConsoleText.text = "connecting... "
        var connectionInfo = new PasswordConnectionInfo(PrivateConstants.host, 22, PrivateConstants.ssh_user, PrivateConstants.ssh_pass);
        ConsoleText.text = "connection infos : ok\n";

        using (var client = new SshClient(connectionInfo))
        {
            ConsoleText.text = "Connecting...\n";
            Debug.Log("Connecting...");
            client.Connect();
            ConsoleText.text += "OK\n";
            connection = true;
            connectionText.text = "SSH: Connected!";
            Debug.Log("SSH Connected");
            while (connection)
            {
                pingCounter++;
                if (pingCounter > 60)
                {
                    pingCounter = 0;
                    try
                    {
                        client.RunCommand("pwd");
                    } catch (System.Exception e)
                    {
                        ConsoleText.text = "ssh disconnected, reconnecting";
                        connection = false;
                        reconnect = true;
                        break;
                    }
                }
                yield return new WaitForSeconds(2);
                if ((pushFile != null && pushFile.Length > 1) && (pushContents != null && pushContents.Length > 1))
                {
                    //writeToFile(pushFile, pushContents);
                    try
                    {
                        try
                        {
                            client.RunCommand("rm " + pushFile);
                        }
                        catch (System.Exception e) { }
                        ConsoleText.text = "Push to file: " + pushFile + "\n" + client.RunCommand("echo \"" + pushContents + "\" >> " + pushFile).Result;
                        pushFile = null;
                        pushContents = null;
                    } catch (System.Exception e)
                    {
                        ConsoleText.text = "ssh disconnected, reconnecting";
                        connection = false;
                        reconnect = true;
                        break;
                    }
                }
                if (pullFile != null && pullFile.Length > 1)
                {
                    try
                    {

                        ConsoleText.text = "Pull from file: " + pullFile;
                        pullContents = client.RunCommand("cat " + pullFile).Result;
                        pullFile = null;
                    } catch (System.Exception e)
                    {
                        ConsoleText.text = "ssh disconnected, reconnecting";
                        connection = false;
                        reconnect = true;
                        break;
                    }
                }
            }
            client.Disconnect();
            connection = false;

            connectionText.text = "SSH: disconnected";

            if (reconnect)
            {
                yield return new WaitForSeconds(0.2f);
                StartCoroutine(Connect());
            }
        } 
    }

    /*
    public void Disconnect()
    {

    }*/
}
