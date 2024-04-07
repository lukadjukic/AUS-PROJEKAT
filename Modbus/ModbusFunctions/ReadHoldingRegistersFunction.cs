using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read holding registers functions/requests.
    /// </summary>
    public class ReadHoldingRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadHoldingRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadHoldingRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            ModbusReadCommandParameters parametri = this.CommandParameters as ModbusReadCommandParameters;
            byte[] zahtev = new byte[12];
            byte[] temp;
            // Pakovanje zahteva:
            temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parametri.TransactionId));
            zahtev[0] = temp[0];
            zahtev[1] = temp[1];
            temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parametri.ProtocolId));
            zahtev[2] = temp[0];
            zahtev[3] = temp[1];
            temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parametri.Length));
            zahtev[4] = temp[0];
            zahtev[5] = temp[1];

            // Vec su bajti
            zahtev[6] = parametri.UnitId;
            zahtev[7] = parametri.FunctionCode;

            temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parametri.StartAddress));
            zahtev[8] = temp[0];
            zahtev[9] = temp[1];
            temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)parametri.Quantity));
            zahtev[10] = temp[0];
            zahtev[11] = temp[1];

            return zahtev;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> recnik = new Dictionary<Tuple<PointType, ushort>, ushort>();
            ModbusReadCommandParameters parametri = CommandParameters as ModbusReadCommandParameters;
            ushort startAdress = parametri.StartAddress;
            byte byteCount = response[8];
            ushort vrednost;
            //Iscitavanje vrednosti 
            for (int i = 0; i < byteCount; i += 2)
            {
                Tuple<PointType, ushort> adresa = new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, startAdress++);
                vrednost = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(response, 9 + i));
                recnik.Add(adresa, vrednost);
            }

            return recnik;
        }
    }
}