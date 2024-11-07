namespace UnitTests;

// ReSharper disable FunctionNeverReturns
using System.Device.I2c;
using System.Device.Spi;
using System.Text;
using Newtonsoft.Json;
using Spectre.Console;
using Exception = System.Exception;

public class Program
{
    private const bool useSPI = true;
    #region Variables

// ReSharper disable once UnusedAutoPropertyAccessor.Global
    public static Program Instance { get; private set; } = null!;

    internal I2cDevice I2cReceiver { get; }
    internal SpiDevice SpiReceiver { get; }

    #endregion

    public static void Main(string[] args)
    {
        Config.LoadConfigs();
        new Program().MainLoop(args);
    }

    private Program()
    {
        Instance = this;
        if(!useSPI)
            this.I2cReceiver = I2cDevice.Create(new I2cConnectionSettings(1, 2));
        else
            this.SpiReceiver = SpiDevice.Create(new SpiConnectionSettings(0, 1));
    }

    // ReSharper disable once UnusedParameter.Local
    private void MainLoop(string[] args)
    {
        Span<byte> buffer = stackalloc byte[6];
        while (true)
        {
            Thread.Sleep(250);
            try
            {
                if (!useSPI)
                    I2cReceiver.Read(buffer);
                else
                    SpiReceiver.Read(buffer);
                if (buffer.Length < 6)
                {
                    AnsiConsole.MarkupLine($"[bold red]Error:[/] No Data less than 6 received ({buffer.Length}).");
                    continue;
                }

                string conc = "";
                if (buffer[0] != 0b00000001 || buffer[5] != 0b00000010)
                {
                    for (int i = 0; i < buffer.Length; i++)
                        conc += $"{buffer[i]}, ";
                    conc = conc.Substring(0, conc.Length -2);
                    AnsiConsole.MarkupLine($"[bold red]Error:[/] Data incorrect headers. ({conc})");
                    continue;
                }
                byte module1 = buffer[1];
                byte module2 = buffer[2];
                byte module3 = buffer[3];
                byte module4 = buffer[4];
                AnsiConsole.MarkupLine($"Data Recieved: [cyan]{module1 / 255}%, {module2 / 255}%, {module3 / 255}%, {module4 / 255}%[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }
    }
}
