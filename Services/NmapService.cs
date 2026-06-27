using System.Diagnostics;
using System.Text.RegularExpressions;
using LogicaServidor.Models.Domains;

namespace LogicaServidor.Services;

public class NmapService
{
    private List<Plc> Plcs { get; set; } = [];

    public List<Plc> GetListaPlcs()
    {
        return Plcs;
    }

    public void EscaneoRed()
    {
        using var processRed = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments =
                    "-c \"ipcalc-ng $(ip -o -f inet addr show eth0 | gawk '{print $4}') | gawk '/Network:/ {print $2}'\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        processRed.Start();

        string nombreRed = processRed.StandardOutput.ReadToEnd().Trim();
        string errorRed = processRed.StandardError.ReadToEnd();

        processRed.WaitForExit();

        if (processRed.ExitCode != 0 || string.IsNullOrWhiteSpace(nombreRed))
            throw new Exception($"Error obteniendo la red: {errorRed}");

        using var processNmap = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "nmap",
                Arguments = $"-sn {nombreRed}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        processNmap.Start();

        string output = processNmap.StandardOutput.ReadToEnd();
        string errorNmap = processNmap.StandardError.ReadToEnd();

        processNmap.WaitForExit();

        if (processNmap.ExitCode != 0)
            throw new Exception($"Error ejecutando nmap: {errorNmap}");

        var regex = new Regex(
            @"Nmap scan report for (?:(?<hostname>.+?) \((?<ip>\d+\.\d+\.\d+\.\d+)\)|(?<iponly>\d+\.\d+\.\d+\.\d+))",
            RegexOptions.Multiline);

        Plcs = regex.Matches(output)
            .Select(m => new Plc
            {
                Ip = m.Groups["ip"].Success
                    ? m.Groups["ip"].Value
                    : m.Groups["iponly"].Value,

                Hostname = m.Groups["hostname"].Success
                    ? m.Groups["hostname"].Value
                    : string.Empty
            })
            .Where(plc => plc.Hostname.StartsWith("plc-tr-", StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}