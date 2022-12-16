using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSmasher : Data
{
    [HideInInspector] public DataStruct data;
    
    public struct DataStruct
    {
        public int ntot, nnopt, nout, nit, nav, ngr, nrelax, ncooling, ndisplace;
        public double hco, hfloor, sep0, tf, dtout, t, alpha, beta, tjumpahead, trelax, dt, omega2, erad, displacex, displacey, displacez;

        public double[] x,
            y,
            z,
            am,
            hp,
            rho,
            vx,
            vy,
            vz,
            vxdot,
            vydot,
            vzdot,
            u,
            udot,
            grpot,
            meanmolecular,
            divv,
            ueq,
            tthermal,
            popacity,
            uraddot,
            temperatures,
            tau;

        public int[] cc;
    }
    
    /// <summary>
    /// Initialize the data arrays for N particles
    /// </summary>
    public override void Read(bool verbose = false)
    {
        
        data.ntot = data.nnopt = data.nout = data.nit = data.nav = data.ngr = data.nrelax = data.ncooling = 
            data.ndisplace = 0;
        data.hco = data.hfloor = data.sep0 = data.tf = data.dtout = data.t = data.alpha = data.beta =
            data.tjumpahead = data.trelax = data.dt = data.omega2 = data.erad = data.displacex = data.displacey =
                data.displacez = 0f;
        if (verbose) Debug.Log("Reading " + file.path);
        using (var stream = System.IO.File.Open(file.path, System.IO.FileMode.Open))
        {
            using (var reader = new System.IO.BinaryReader(stream, System.Text.Encoding.UTF8, false))
            {
                long fileLength = reader.BaseStream.Length;
                uint record = reader.ReadUInt32(); // Fortran record length header
                data.ntot = FileReader.ReadBinary<int>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.nnopt = FileReader.ReadBinary<int>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.hco = FileReader.ReadBinary<double>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.hfloor = FileReader.ReadBinary<double>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.sep0 = FileReader.ReadBinary<double>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.tf = FileReader.ReadBinary<double>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.dtout = FileReader.ReadBinary<double>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.nout = FileReader.ReadBinary<int>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.nit = FileReader.ReadBinary<int>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.t = FileReader.ReadBinary<double>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.nav = FileReader.ReadBinary<int>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.alpha = FileReader.ReadBinary<double>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.beta = FileReader.ReadBinary<double>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.tjumpahead = FileReader.ReadBinary<double>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.ngr = FileReader.ReadBinary<int>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.nrelax = FileReader.ReadBinary<int>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.trelax = FileReader.ReadBinary<double>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.dt = FileReader.ReadBinary<double>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.omega2 = FileReader.ReadBinary<double>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.ncooling = FileReader.ReadBinary<int>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.erad = FileReader.ReadBinary<double>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.ndisplace = FileReader.ReadBinary<int>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.displacex = FileReader.ReadBinary<double>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.displacey = FileReader.ReadBinary<double>(reader, ref record);
                if (record <= 0) goto header_finished;
                data.displacez = FileReader.ReadBinary<double>(reader, ref record);
                
                header_finished:
                {
                    if (verbose)
                    {
                        Debug.Log("Header: "+data.ntot + " " + data.nnopt + " " + data.hco + " " + data.hfloor + " " + 
                                  data.sep0 + " " + data.tf + " " + data.dtout + " " +
                                  data.nout + " " + data.nit + " " + data.t + " " + data.nav + " " + data.alpha + " " + 
                                  data.beta + " " + data.tjumpahead + " " + data.ngr + " " + data.nrelax + " " + 
                                  data.trelax + " " + data.dt + " " + data.omega2 + " " + data.ncooling + " " + 
                                  data.erad + " " + data.ndisplace + " " + data.displacex + " " + data.displacey + " " +
                                  data.displacez);
                    }
                }
                reader.ReadUInt32(); // Fortran record length footer

                
                // Initialize the data arrays
                data.x = new double[data.ntot];
                data.y = new double[data.ntot];
                data.z = new double[data.ntot];
                data.am = new double[data.ntot];
                data.hp = new double[data.ntot];
                data.rho = new double[data.ntot];
                data.vx = new double[data.ntot];
                data.vy = new double[data.ntot];
                data.vz = new double[data.ntot];
                data.vxdot = new double[data.ntot];
                data.vydot = new double[data.ntot];
                data.vzdot = new double[data.ntot];
                data.u = new double[data.ntot];
                data.udot = new double[data.ntot];
                data.grpot = new double[data.ntot];
                data.meanmolecular = new double[data.ntot];
                data.cc = new int[data.ntot];
                data.divv = new double[data.ntot];
                data.ueq = new double[data.ntot];
                data.tthermal = new double[data.ntot];
                data.popacity = new double[data.ntot];
                data.uraddot = new double[data.ntot];
                data.temperatures = new double[data.ntot];
                data.tau = new double[data.ntot];

                // Read each line
                for (int i = 0; i < data.ntot; i++)
                {
                    record = reader.ReadUInt32(); // Fortran record length header
                    data.x[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.y[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.z[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.am[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.hp[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.rho[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.vx[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.vy[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.vz[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.vxdot[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.vydot[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.vzdot[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.u[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.udot[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.grpot[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.meanmolecular[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.cc[i] = FileReader.ReadBinary<int>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.divv[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.ueq[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.tthermal[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.popacity[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.uraddot[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.temperatures[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    data.tau[i] = FileReader.ReadBinary<double>(reader, ref record);
                    if (record <= 0) goto line_finished;
                    line_finished: { reader.ReadUInt32(); } // Fortran record length footer
                }
                
                record = reader.ReadUInt32(); // Fortran record length header
                int check = FileReader.ReadBinary<int>(reader);
                record = reader.ReadUInt32(); // Fortran record length footer
                
                if (check != data.ntot) throw new System.Exception("Corrupt file?");
                if (reader.BaseStream.Position != fileLength)
                    throw new System.Exception(
                        "Failed to read the entire file. This likely means the file is corrupt.");
            }
        }

        nParticles = data.ntot;
    }

    public override Vector3[] GetPositions()
    {
        Vector3[] positions = new Vector3[data.ntot];
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i].x = (float)data.x[i];
            positions[i].y = (float)data.y[i];
            positions[i].z = (float)data.z[i];
        }
        return positions;
    }

    public override float[] GetSizes()
    {
        float[] sizes = new float[data.ntot];
        for (int i = 0; i < sizes.Length; i++) sizes[i] = 2f * (float)data.hp[i];
        return sizes;
    }
}
