using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTelemetryEntities.Enums
{
        public enum Estacion
        {
            Chasis,
            Motor,
            Ensamblado,
            Pintura,
            Electricidad,
            Control_Calidad
        }
        public enum EstadoTelemetria
        {
            Pendiente,
            Enviando,
            Procesado,
            Alerta_Temperatura,
            Error_Duplicado,
            Error_HTTP,
            Error_Conexion
        }   
}
