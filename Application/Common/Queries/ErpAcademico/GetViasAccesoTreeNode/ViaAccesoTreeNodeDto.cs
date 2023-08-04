using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Queries.ErpAcademico.GetViasAccesoTreeNode
{
    public class ViaAccesoTreeNodeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Selectable { get; set; }
        public List<ViaAccesoTreeNodeDto> Nodes { get; set; }
    }
}
