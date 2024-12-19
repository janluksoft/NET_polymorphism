//==== Author: janluksoft@interia.pl ==========================
// See https://aka.ms/new-console-template for more information

using System.Xml.Linq;

//========== Program FlyingMachine =======================

  Console.WriteLine("Hello, Aircrafts!"); 
  Console.WriteLine("Below are the parameters of several FlyingMachines and their calculated lift:\n\r");

  string name = "Airbus A320";
  double weight = 78000;    // kg
  double airDensity = 0.38; // kg/m^3 on 10 km;  1,225 kg/m^3 on the ground
  double velocity = 230;    // m/s
  double wingArea = 122.6;  // m^2

  List<FlyingMachine> machines = new List<FlyingMachine>
  {
    //               name, weight, airDensity, velocity, wingArea, liftCoefficient
    //new JetPlane(     "Airbus A320", 78000, 1.225, 250  , 122.6, 0.62),
    new JetPlane(       "Airbus A320", 78000, 0.38 , 230  , 122.6, 0.62),
    new JetPlane(    "Boeing 737-800", 79000, 0.38 , 230  , 124.6, 0.62),

    new Helicopter("UH-60 Black Hawk", 10660, 1.225, 8.18 , 16.02, 0.22),
    new Helicopter("Eurocopter EC135",  2980, 1.225, 5.1  , 22.12, 0.18),
    //new Helicopter("UH-60 Black Hawk", 10660, 1.225, 8.18 , 27.02, 0.6),

    new HotAirBalloon("Cameron Z-750",   3640,21238 , 1.225, 0.925),
    new HotAirBalloon("Ultramagic N-425",1930,12100 , 1.225, 0.925),
    new HotAirBalloon("Cameron Z-160",    650, 4531 , 1.225, 0.925)
  };

  foreach (var machine in machines)
  {
    Console.WriteLine(machine.FlyingMessage());
  }

//========== End FlyingMachine =======================

//===== Interface for calculating lift =====
public interface ILiftCalculator
{
    double CalculateLift();
}

public interface IMessageFormat
{
    string fMakeMessage(string xKind, string xName, float xMass, double xLiftForce)
    {
        double liftForce_kG = xLiftForce/ 9.81;

        string smess = $" {FixedString(xKind, 11)} " +

            $" {FixedString(xName, 16)}" +
            $" mass:{xMass,  9:### ### ###.0} kg" +
            $"  lift force:{liftForce_kG,10:### ### ###.0} kG";
        return (smess);
    }

    string FixedString(string xmessage, int xLen)
    {
        string mess = xmessage + "                       ";
        mess = mess.Substring(0, xLen);
        return (mess);
    }
}


// ordinaty class for air parameters
public class CAir 
{
    public double AirDensity { get; protected set; } // in kg/m^3 ambientAirDensity
    public double HeatedAirDensity { get; protected set; } // in kg/m^3
    protected CAir(double airDensity, double heatedAirDensity = 0)
    {
        this.AirDensity = airDensity; //double heatedAirDensity;
        this.HeatedAirDensity = heatedAirDensity;
    }
    public (double airCold, double airHot) GetAirDensity() //System.ValueTuple.dll
    {
        return (AirDensity, HeatedAirDensity);
    }
}

// Abstract base class for flying machines
public abstract class FlyingMachine : CAir, ILiftCalculator, IMessageFormat
{
    public string Name { get; protected set; }
    public double Weight { get; protected set; } // in kg
    protected FlyingMachine(string name, double weight, double airDensity) : base(airDensity)
    {
        Name = name;
        Weight = weight;
    }
    protected FlyingMachine(string name, double weight, double airDensity, double heatedAirDensity) 
                     : base(airDensity, heatedAirDensity)
    {
        Name = name;
        Weight = weight;
    }

    public abstract double CalculateLift(); // Must be implemented by subclasses
    public abstract string FlyingMessage();// Must be implemented by subclasses
}

// Jet Plane class
public class JetPlane : FlyingMachine
{
    private double velocity;
    private double wingArea;
    private double liftCoefficient;

    public JetPlane(string name, double weight, double airDensity, double velocity,
                    double wingArea, double liftCoefficient): base(name, weight, airDensity)
    {
        this.velocity = velocity;
        this.wingArea = wingArea;
        this.liftCoefficient = liftCoefficient;
    }

    public override string FlyingMessage()
    {
        //double airDensityTest = GetAirDensity(); //test Work from class CAir
        string sMess = (this as IMessageFormat).fMakeMessage("JetPlane", Name, (float)Weight, CalculateLift());
        return (sMess);
    }

    public override double CalculateLift()
    {
        return 0.5 * AirDensity * velocity * velocity * wingArea * liftCoefficient;
    }
}

// Helicopter class
public class Helicopter : FlyingMachine
{
    private double rotorRadius;
    private double angularVelocity;
    private double liftCoefficient;

    public Helicopter(string name, double weight, double airDensity,
                      double rotorRadius, double angularVelocity, double liftCoefficient)
        : base(name, weight, airDensity)
    {
        this.rotorRadius = rotorRadius;
        this.angularVelocity = angularVelocity;
        this.liftCoefficient = liftCoefficient;
    }

    public override string FlyingMessage()
    {   //This sMess variable is convenient for debugging
        string sMess = (this as IMessageFormat).fMakeMessage("Heli ground", Name, (float)Weight, CalculateLift());
        return (sMess);
    }

    public override double CalculateLift()
          {//new Helicopter("UH-60 Black Hawk", 10660, 1.225, 8.18 , 27.02, 0.6),
        double lift = liftCoefficient * (2.0 / 9.0) * AirDensity * Math.PI * Math.Pow(rotorRadius, 4) * Math.Pow(angularVelocity, 2);
        return (lift);
    }
}

// Hot Air Balloon class
public class HotAirBalloon : FlyingMachine
{
    private double volume;
    //private double ambientAirDensity;
    //private double heatedAirDensity;

    public HotAirBalloon(string name, double weight, double volume, double ambientAirDensity, double heatedAirDensity)
        : base(name, weight, ambientAirDensity, heatedAirDensity) //base = CAir(double airDensity, double heatedAirDensity = 0)
    {
        this.volume = volume;
        //class Air capture air parameters. Parameters bellow are not need
        //this.ambientAirDensity = ambientAirDensity;
        //this.heatedAirDensity = heatedAirDensity;
    }

    public override string FlyingMessage()
    {   //This sMess variable is convenient for debugging
        string sMess = (this as IMessageFormat).fMakeMessage("Balloon", Name, (float)Weight, CalculateLift());
        return (sMess);
    }

    public override double CalculateLift()
    {
        //(double airCold, double airHot) GetAirDensity()
        var tupleAir = GetAirDensity(); //Use tuple type
        double balloonLift = volume * (tupleAir.airCold - tupleAir.airHot) * 9.81;
        return (balloonLift);
        //return volume * (ambientAirDensity - heatedAirDensity) * 9.81; // Gravity factor included
    }
}


