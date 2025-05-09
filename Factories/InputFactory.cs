using DVG.SkyPirates.Server.IFactories;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Server.Presenters;

namespace DVG.SkyPirates.Server.Factories
{
    internal class InputFactory: IInputFactory
    {
        private readonly IInputService _inputService;

        public InputFactory(IInputService inputService)
        {
            _inputService = inputService;
        }

        public InputPm Create(int clientId)
        {
            var input = new InputPm();
            _inputService.RegisterInput(input, clientId);
            return input;
        }
    }
}
