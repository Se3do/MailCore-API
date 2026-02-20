using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailService.Application.Commands.Labels.UnassignLabel
{
    public record UnassignLabelCommand(Guid userId, Guid mailId, Guid labelId);
}
