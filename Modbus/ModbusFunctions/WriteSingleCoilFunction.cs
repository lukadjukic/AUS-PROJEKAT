using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write coil functions/requests.
    /// </summary>
    public class WriteSingleCoilFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleCoilFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            ModbusWriteCommandParameters parametri = CommandParameters as ModbusWriteCommandParameters;
            byte[] zahtev = new byte[12];
            byte[] temp;
            //Pakovanje zahteva
            temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parametri.TransactionId));
            zahtev[0] = temp[0];
            zahtev[1] = temp[1];
            temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parametri.ProtocolId));
            zahtev[2] = temp[0];
            zahtev[3] = temp[1];
            temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parametri.Length));
            zahtev[4] = temp[0];
            zahtev[5] = temp[1];

            //Vec je byte
            zahtev[6] = parametri.UnitId;
            zahtev[7] = parametri.FunctionCode;

            temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parametri.OutputAddress));
            zahtev[8] = temp[0];
            zahtev[9] = temp[1];
            temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parametri.Value));
            zahtev[10] = temp[0];
            zahtev[11] = temp[1];

            return zahtev;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> recnik = new Dictionary<Tuple<PointType, ushort>, ushort>();
            ushort regNum = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(response, 8));
            ushort vrednost = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(response, 10));
            Tuple<PointType, ushort> adresa = new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, regNum);
            recnik.Add(adresa, vrednost);
            return recnik;
        }
    }
}