using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailService.Application.Queries.Labels.GetAllLabels
{
    public record GetAllLabelsQuery(Guid UserId);
}
