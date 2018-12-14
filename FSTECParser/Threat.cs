using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSTECParser
{
    class Threat
    {
        public int IdThreat { get; set; }
        public string ThreatName { get; set; }
        public string ThreatDescription { get; set; }
        public string ThreatSource { get; set; }
        public string ThreatObject { get; set; }
        public bool Confidentiality { get; set; }
        public bool Integrity { get; set; }
        public bool Availability { get; set; }

        public Threat(int idThreat, string threatName, string threatDescription, string threatSource, string threatObject, bool confidentiality, bool integrity, bool availability)
        {
            IdThreat = idThreat;
            ThreatName = threatName;
            ThreatDescription = threatDescription;
            ThreatSource = threatSource;
            ThreatObject = threatObject;
            Confidentiality = confidentiality;
            Integrity = integrity;
            Availability = availability;
        }

        public override string ToString()
        {
            string confidentiality = "", integrity = "", availability = "";
            if (Confidentiality)
                confidentiality = "Yes";
            else
                confidentiality = "No";
            if (Integrity)
                integrity = "Yes";
            else
                integrity = "No";
            if (Availability)
                availability = "Yes";
            else
                availability = "No";
            return IdThreat + " " + ThreatName + " " + ThreatSource + " " + ThreatObject + " " +
                    confidentiality + " " + integrity + " " + availability;
        }

        public override bool Equals(object obj)
        {
            var threat = obj as Threat;
            return threat != null &&
                   IdThreat == threat.IdThreat &&
                   ThreatName == threat.ThreatName &&
                   ThreatDescription == threat.ThreatDescription &&
                   ThreatSource == threat.ThreatSource &&
                   ThreatObject == threat.ThreatObject &&
                   Confidentiality == threat.Confidentiality &&
                   Integrity == threat.Integrity &&
                   Availability == threat.Availability;
        }

        public override int GetHashCode()
        {
            var hashCode = 724792092;
            hashCode = hashCode * -1521134295 + IdThreat.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ThreatName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ThreatDescription);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ThreatSource);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ThreatObject);
            hashCode = hashCode * -1521134295 + Confidentiality.GetHashCode();
            hashCode = hashCode * -1521134295 + Integrity.GetHashCode();
            hashCode = hashCode * -1521134295 + Availability.GetHashCode();
            return hashCode;
        }
    }
}
