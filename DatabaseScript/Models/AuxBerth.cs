using System;
using System.Collections.Generic;

namespace DatabaseScript.Models;

public partial class AuxBerth
{
    public uint IdBerth { get; set; }

    public string Berth { get; set; } = null!;

    /// <summary>
    /// Ma ajuta sa imi dau seama care dana pe unde este in port, pentru dane cu nume de genul &quot;Doua Dane&quot;, &quot;Gabare&quot;, &quot;Port lucru&quot;. Ia numele celei mai apropiate dane numerice. Ex: Doua Dane ar trebui sa fie -2, Dana Gabare -1, RoRo1 0.25, roro4: 0.75
    /// </summary>
    public string Poz { get; set; } = null!;
}
