using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Hardware;

namespace ArduinoHWMonitor {
    class Program {
        // Member data
        Computer thisComputer;                              // To store the reference to this computer
        SerialPort serPort;                                 // To store connection to serial port
        
        // Temperature stats
        String cpuTemp;                                     // To store CPU temperature
        String gpuTemp;                                     // To store GPU temperature

        // Usage stats
        String cpuUsage;                                    // To store CPU usage
        String gpuUsage;                                    // To store GPU usage

        // Member method
        // Constructor
        Program() {
            // Creating new connection to get GPU and CPU data
            thisComputer = new Computer() { CPUEnabled = true, GPUEnabled = true };

            // Creating connection to SerialPort
            serPort = new SerialPort();
            serPort.PortName = "COM3";                      // Setting port name
            serPort.BaudRate = 500000;                      // Setting baud rate

            // Opening port
            serPort.Open();

            // Opening connection
            thisComputer.Open();
        }

        // Method to pad stats to 3 digits
        private String padStat(String stat) {
            if(stat.Length == 1) {
                return "  " + stat;
            }
            else if(stat.Length == 2) {
                return " " + stat;
            }
            else if(stat.Length == 3) {
                return stat;
            }
            return "   ";
        }

        // Method to get CPU and GPU stats
        private String getStats() {
            // Getting each hardware item
            foreach(var hwItem in thisComputer.Hardware) {
                // Checking if hardware is CPU
                if(hwItem.HardwareType == HardwareType.CPU) {
                    // Updating to get newest stats
                    hwItem.Update();
                    // Getting each stat
                    foreach(var sensor in hwItem.Sensors) {
                        // For temp
                        if(sensor.SensorType == SensorType.Temperature) {
                            cpuTemp = "" + (int)Math.Round(sensor.Value.Value);
                        }
                        // For usage
                        else if(sensor.SensorType == SensorType.Load) {
                            cpuUsage = "" + (int)Math.Round(sensor.Value.Value);
                        }
                    }
                }
                // Checking if hardware is GPU
                else if (hwItem.HardwareType == HardwareType.GpuNvidia) {
                    // Updating to get newest stats
                    hwItem.Update();
                    // Getting each stat
                    foreach (var sensor in hwItem.Sensors) {
                        // For temp
                        if (sensor.SensorType == SensorType.Temperature) {
                            gpuTemp = "" + (int)Math.Round(sensor.Value.Value);
                        }
                        // For usage
                        else if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Core") {
                            gpuUsage = "" + (int)Math.Round(sensor.Value.Value);
                        }
                    }
                }
            }

            // Padding stats
            cpuTemp = padStat(cpuTemp);
            gpuTemp = padStat(gpuTemp);

            cpuUsage = padStat(cpuUsage);
            gpuUsage = padStat(gpuUsage);

            // Creating combined stat
            String stats = "CPU : " + cpuUsage + "%" + "  " + cpuTemp + "C" + "\nGPU : " + gpuUsage + "%" + "  " + gpuTemp + "C";
            return stats;
        }

        // Method to relay stats through serial port
        private void sendStats(String stats) {
            serPort.WriteLine(stats);
        }

        // Main method
        public static void Main(String[] args) {
            Program ob = new Program();
            while (true) {
                String stats = ob.getStats();
                Console.WriteLine(stats);
                ob.sendStats(stats);
                Thread.Sleep(1000);
            }
        }
    }
}
