using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using CoreBot.DialogDetails;
using CoreBot.Helpers;
using CoreBot.Models;
using Data.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace CoreBot.Dialogs;

public class RequestPrescriptionDialog : ComponentDialog
{
    private readonly PrescriptionDataService _prescriptionDataService;
    private readonly MedicineDataService _medicineDataService;

    private const string NameMessage = "What name can I put that under?";
    private const string ReasonMessage = "For what reason do you a need prescription?";
    private const string MedicineMessage = "Which medicine do you need?";

    private const string OkMessage = "We have received your request. A medical professional will take a look at it shortly.";

    public RequestPrescriptionDialog(PrescriptionDataService prescriptionDataService, MedicineDataService medicineDataService) : base(nameof(RequestPrescriptionDialog))
    {
        _prescriptionDataService = prescriptionDataService;
        _medicineDataService = medicineDataService;

        AddDialog(new TextPrompt(nameof(TextPrompt)));
        AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

        var waterfallSteps = new WaterfallStep[]
        {
            FirstNameStepAsync,
            NameReasonStepAsync,
            ReasonMedicineStepAsync,
            MedicineActStepAsync,
        };

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
        InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> FirstNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var details = (RequestPrescriptionDetails)stepContext.Options;

        if (details.Name == null)
        {
            var promptMessage = MessageFactory.Text(NameMessage, NameMessage, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        return await stepContext.NextAsync(details.Name, cancellationToken);
    }

    private async Task<DialogTurnResult> NameReasonStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var details = (RequestPrescriptionDetails)stepContext.Options;
        details.Name = (string)stepContext.Result;

        if (details.Reason == null)
        {
            var promptMessage = MessageFactory.Text(ReasonMessage, ReasonMessage, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        return await stepContext.NextAsync(details.Reason, cancellationToken);
    }

    private async Task<DialogTurnResult> ReasonMedicineStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var details = (RequestPrescriptionDetails)stepContext.Options;
        details.Reason = (string)stepContext.Result;

        var medicines = await _medicineDataService.GetMedicineAsync();

        var medicineList = medicines.Select(m => new AdaptiveSubmitAction
        {
            Title = m.Name,
            Data = m.Id.ToString()
        }).ToList();

        return await ChoicePromptHelper.PromptChoiceAsync(MedicineMessage, medicineList, stepContext, cancellationToken);
    }

    private async Task<DialogTurnResult> MedicineActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var details = (RequestPrescriptionDetails)stepContext.Options;
        details.MedicineId = int.Parse(((FoundChoice)stepContext.Result).Value);

        await _prescriptionDataService.InsertPrescriptionAsync(new Prescription
        {
            PatientName = details.Name,
            Reason = details.Reason,
            MedicineId = details.MedicineId
        });
        await stepContext.Context.SendActivityAsync(MessageFactory.Text(OkMessage), cancellationToken);

        return await stepContext.EndDialogAsync(details, cancellationToken);
    }
}