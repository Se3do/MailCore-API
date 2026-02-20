using MailService.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailService.Application.Queries.Mailbox.GetByLabelPaged
{
    public record GetByLabelPagedQuery(Guid UserId, Guid LabelId, CursorPaginationQuery Pagination);
}
