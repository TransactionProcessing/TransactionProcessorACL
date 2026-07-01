using System.Threading;
using System.Threading.Tasks;

namespace TransactionProcessorACL.Common;

public interface IRequestAuditRecorder
{
    Task RecordAsync(RequestAuditEvent auditEvent, CancellationToken cancellationToken);
}
