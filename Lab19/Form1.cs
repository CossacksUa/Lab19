using System;
using System.Collections.Generic;
using System.Management;
using System.Windows.Forms;
using OpenHardwareMonitor.Hardware;

namespace Lab19
{
    public partial class Form1 : Form
    {
        private Computer computer;

        public Form1()
        {
            InitializeComponent();

            computer = new Computer()
            {
                CPUEnabled = true,
                GPUEnabled = true
            };
            computer.Open();

            // Подключаем обработчик события для кнопки
            btnGetInfo.Click += new EventHandler(btnGetInfo_Click);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Ініціалізація форми
        }

        private void btnGetInfo_Click(object sender, EventArgs e)
        {
            // Очищення виводу перед новим запитом
            txtOutput.Clear();

            AppendResult("Процесор:", GetHardwareInfo("Win32_Processor", "Name"));
            AppendResult("Виробник процесора:", GetHardwareInfo("Win32_Processor", "Manufacturer"));
            AppendResult("Опис процесора:", GetHardwareInfo("Win32_Processor", "Description"));
            txtOutput.AppendText(Environment.NewLine);

            AppendResult("Відеокарта:", GetHardwareInfo("Win32_VideoController", "Name"));
            AppendResult("Видеопроцесор:", GetHardwareInfo("Win32_VideoController", "VideoProcessor"));
            AppendResult("Версія драйверу відеокарти:", GetHardwareInfo("Win32_VideoController", "DriverVersion"));
            AppendResult("Об’єм пам’яти відеокарти (в байтах):", GetHardwareInfo("Win32_VideoController", "AdapterRAM"));
            txtOutput.AppendText(Environment.NewLine);

            AppendTemperatureInfo();

            txtOutput.AppendText(Environment.NewLine);
            AppendResult("Назва DVD:", GetHardwareInfo("Win32_CDROMDrive", "Name"));
            AppendResult("Буква DVD:", GetHardwareInfo("Win32_CDROMDrive", "Drive"));
            txtOutput.AppendText(Environment.NewLine);

            AppendResult("Жорстикий диск:", GetHardwareInfo("Win32_DiskDrive", "Caption"));
            AppendResult("Об’єм жорстикого диску (в байтах):", GetHardwareInfo("Win32_DiskDrive", "Size"));
            txtOutput.AppendText(Environment.NewLine);

            AppendResult("Материнська плата:", GetHardwareInfo("Win32_BaseBoard", "Product"));
            AppendResult("Виробник материнської плати:", GetHardwareInfo("Win32_BaseBoard", "Manufacturer"));
            txtOutput.AppendText(Environment.NewLine);

            AppendResult("Мережеве обладнання:", GetHardwareInfo("Win32_NetworkAdapter", "Name"));
            AppendResult("MAC адреса:", GetHardwareInfo("Win32_NetworkAdapter", "MACAddress"));
            txtOutput.AppendText(Environment.NewLine);

            AppendResult("BIOS версія:", GetHardwareInfo("Win32_BIOS", "Version"));
            AppendResult("BIOS виробник:", GetHardwareInfo("Win32_BIOS", "Manufacturer"));
            AppendResult("BIOS опис:", GetHardwareInfo("Win32_BIOS", "Description"));
            txtOutput.AppendText(Environment.NewLine);

            
        }

        private List<string> GetHardwareInfo(string WIN32_Class, string ClassItemField)
        {
            List<string> result = new List<string>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM " + WIN32_Class);
            try
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    result.Add(obj[ClassItemField]?.ToString().Trim() ?? "N/A");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        private void AppendResult(string info, List<string> result)
        {
            if (!string.IsNullOrEmpty(info))
            {
                txtOutput.AppendText(info + Environment.NewLine);
            }
            if (result.Count > 0)
            {
                foreach (var item in result)
                {
                    txtOutput.AppendText(item + Environment.NewLine);
                }
            }
        }

        private void AppendTemperatureInfo()
        {
            foreach (var hardwareItem in computer.Hardware)
            {
                hardwareItem.Update();
                foreach (var sensor in hardwareItem.Sensors)
                {
                    if (sensor.SensorType == SensorType.Temperature)
                    {
                        txtOutput.AppendText($"{sensor.Name}: {sensor.Value.GetValueOrDefault()} °C" + Environment.NewLine);
                    }
                }
            }
        }
    }
}
