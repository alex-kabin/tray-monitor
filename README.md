## Tray Monitor

Utility for monitoring various sensors and displaying results as tray icon indicators. 
Written with C# (+WinForms) and requires .NET Framework 4.7.2+ to run.

The utility is portable (has not installer) and expects to be started with command line arguments:

    [-c|--connect] -s|--sensor \"SensorType\" \"Key=Value\"* -i|--indicator \"IndicatorType\" \"Key=Value\"*"

where:
* optional `-c` or `--connect` makes sensor to autoconnect after run;
* `-s` or `--sensor` specifies assembly qualified name of SensorType. Also there could be some "Key=Value" pairs with sensor configuration settings;
* `-i` or `--indicator` specifies assembly qualified name of IndicatorType. Also there could be some "Key=Value" pairs with indicator configuration settings;

After successful start there will be indicator icon in system tray. 
Tray icon has context menu with log view option to diagnose problems.

##### Currently implemented sensors and indicators:
* _Bluetooth HandsFree battery monitor_ -> BatteryTrayIndicator
* _Spring Actuator Health endpoint monitor_ -> LampTrayIndicator

#### How to use Bluetooth HandsFree battery monitor:
Bluetooth HandsFree battery sensor uses HF Indicators feature as described in [HFP V1.7](https://www.bluetooth.org/docman/handlers/downloaddoc.ashx?doc_id=292287) and Apple Specific features from [Apple Accessory Design Guidelines](https://developer.apple.com/accessories/Accessory-Design-Guidelines.pdf)   

First, stop Windows Service '**BTAGService**' (Bluetooth Audio Gateway Service) as it prevents connection to HandsFree device.
_Unfortunately, after this, your bluetooth device will not be usable as voice (HandsFree) device, but it can still be able to play audio or music._  

Start TrayMonitor from command line (or shortcut link) with arguments: 

    -c -s Sensor.HandsFreeBattery.HandsFreeBatterySensor,Sensor.HandsFreeBattery "DeviceName=Mpow M5" -i TrayMonitor.Indicators.BatteryTrayIndicator,TrayMonitor
    
where `DeviceName` is the name of the paired handsfree bluetooth device to monitor.

After successful start there will be battery icon in the system tray with battery charge level or error mark. Tray icon has context menu with log view option to diagnose problems.

#### How to use Spring Boot Actuator health monitor:
Start TrayMonitor from command line (or shortcut link) with arguments: 

    -c -s Sensor.SpringActuatorHealth.SpringActuatorHealthSensor,Sensor.SpringActuatorHealth "URL=http://localhost:8080/actuator/health" "Period=00:00:10" -i TrayMonitor.Indicators.LampTrayIndicator,TrayMonitor
    
where `URL` is url of the health endpoint, and `Period` is polling period (in TimeSpan format) 

After successful start there will be lamp icon in the system tray with health status (GREEN - up, RED - down) or error mark. Tray icon has context menu with log view option to diagnose problems.

#### How to make custom Sensor or Indicator:
* You can implement your custom sensor by subclassing `Sensor.Core.SensorBase` class
* You can implement your custom indicator by subclassing `TrayMonitor.Core.Indicators.TrayIndicator` class

See sources for details and examples.
