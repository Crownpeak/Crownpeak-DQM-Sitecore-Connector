using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Crownpeak.Sitecore.DQM.Service;
using Sitecore.Data.Validators;

namespace Crownpeak.Sitecore.DQM.Logic
{
    public abstract class AbstractValidator : StandardValidator
    {
        private readonly CrownpeakClient _client = new CrownpeakClient(Settings.ApiKey, Settings.RetryCount);

        protected AbstractValidator()
        {
        }

        protected AbstractValidator(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string CheckpointId { get; set; }

        protected abstract string GetErrorText(int totalErrors, int totalCheckpoints);

        public abstract void GetContent(ValidationArgs args);

        public void SendContent(ValidationArgs args)
        {
            var asset = _client.CreateAsset(Settings.WebsiteId, args.Content, "text/html");
            args.AssetId = asset.Id;
        }

        public void GetCheckpoints(ValidationArgs args)
        {
            var status = _client.GetAssetStatus(args.AssetId);
            args.TotalCheckpoints = status.TotalCheckpoints;
            args.TotalErrors = status.TotalErrors;
            args.List = status.Checkpoints;
        }

        public void GetHighlights(ValidationArgs args, bool source)
        {
            if (source)
            {
                args.Source = new [] { _client.GetAssetError(args.AssetId, CheckpointId, true) };
            }
            else
            {
                args.Preview = _client.GetPageHighlight(args.AssetId);
            }
        }

        public void Cleanup(ValidationArgs args)
        {
            _client.DeleteAsset(args.AssetId);
            args.AssetId = null;
        }

        protected override ValidatorResult Evaluate()
        {
            try
            {
                var args = new ValidationArgs();
                GetContent(args);

                if (string.IsNullOrWhiteSpace(args.Content))
                {
                    return ValidatorResult.Valid;
                }

                SendContent(args);
                GetCheckpoints(args);
                Cleanup(args);

                if (args.TotalErrors == 0)
                {
                    return ValidatorResult.Valid;
                }

                var failed = 0;
                foreach (var checkpoint in args.List.Where(c => c.Failed))
                {
                    failed++;
                    var name = HttpUtility.HtmlEncode(checkpoint.Name.Replace("<br/>", string.Empty));
                    Errors.Add(string.Format("{0}. {1}", checkpoint.Reference, name));
                }

                Text = GetErrorText(failed, args.List.Length);
                return GetFailedResult(ValidatorResult.Error);
            }
            catch (Exception ex)
            {
                Text = ex.Message;
                return ValidatorResult.Warning;
            }
        }

        protected override ValidatorResult GetMaxValidatorResult()
        {
            return GetFailedResult(ValidatorResult.Error);
        }
    }
}
