﻿using System;
using System.Collections.Generic;
using Unir.Expedientes.Application.Bus.Messages.Common;

namespace Unir.Expedientes.Application.Bus.Messages
{
    public class MatriculaAmpliacionRecuperada : MatriculaAmpliacionBaseMessage
    {
        public DateTime FechaHora { get; set; }
        public IEnumerable<int> IdsAsignaturasOfertadasAdicionadas { get; set; }
    }
}
