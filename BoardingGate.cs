//==========================================================
// Student Number : S10266942
// Student Name :  Pugazhenthi Dharundev
// Partner Name : T Venkatesh
//==========================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s10266695_s10266942_prg2_assignment;

internal class BoardingGate
{
    private string gateName;
    public string GateName
    {
        get { return gateName; }
        set { gateName = value; }
    }

    private bool supportsCFFT;
    public bool SupportsCFFT
    {
        get { return supportsCFFT; }
        set { supportsCFFT = value; }
    }

    private bool supportsDDJB;
    public bool SupportsDDJB
    {
        get { return supportsDDJB; }
        set { supportsDDJB = value; }
    }

    private bool supportsLWTT;
    public bool SupportsLWTT
    {
        get { return supportsLWTT; }
        set { supportsLWTT = value; }
    }

    private Flight flight;
    public Flight Flight
    {
        get { return flight; }
        set { flight = value; }
    }

    public BoardingGate() { }

    public BoardingGate(string gateName, bool supportsCFFT, bool supportsDDJB, bool supportsLWTT, Flight flight)

    {

        GateName = gateName;
        SupportsCFFT = supportsCFFT;
        SupportsDDJB = supportsDDJB;
        SupportsLWTT = supportsLWTT;
        Flight = flight;
    }


    // ... other properties and methods ...

    public double CalculateFees()
    {
        double fees = 300; // Base boarding gate fee

        if (flight == null)
            return fees;

        // Add fees based on special request codes
        if (supportsDDJB && flight is DDJBFlight)
        {
            fees += 300; // DDJB code request fee
        }

        if (supportsCFFT && flight is CFFTFlight)
        {
            fees += 150; // CFFT code request fee
        }

        if (supportsLWTT && flight is LWTTFlight)
        {
            fees += 500; // LWTT code request fee
        }

        return fees;
    }


    public override string ToString()
    {
        return "Gate Name : " + GateName + "\n" + "Supports CFFT : " + SupportsCFFT + "\n" + "Supports DDJB : " + SupportsDDJB + "\n" + "Supports LWTT : " + SupportsLWTT + "\n" + "Flight : " + Flight;
    }
}
