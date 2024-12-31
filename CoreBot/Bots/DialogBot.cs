// Generated with CoreBot .NET Template version v4.22.0

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace CoreBot.Bots;

public class DialogBot<T> : ActivityHandler
    where T : Dialog
{
    private readonly Dialog _dialog;
    private readonly BotState _conversationState;
    private readonly BotState _userState;
    private readonly ILogger _logger;

    public DialogBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger)
    {
        _conversationState = conversationState;
        _userState = userState;
        _dialog = dialog;
        _logger = logger;
    }

    public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
    {
        await base.OnTurnAsync(turnContext, cancellationToken);

        // Save any state changes that might have occurred during the turn.
        await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
    }

    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Running dialog with Message Activity.");

        // Run the Dialog with the new message Activity.
        await _dialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
    }

    protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
        foreach (var member in membersAdded)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                await _dialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
            }
        }
    }
}