using System.Speech.Synthesis;

namespace KarelazisBot.Modules
{
    public class TextToSpeech
    {
        public TextToSpeech(Objects.Client c)
        {
            this.Client = c;
            this.SpeechSynthesizer = new SpeechSynthesizer();
        }

        public Objects.Client Client { get; private set; }
        public SpeechSynthesizer SpeechSynthesizer { get; private set; }

        public void Speak(string text, bool flashClient)
        {
            if (flashClient) WinAPI.FlashWindow(this.Client.TibiaProcess.MainWindowHandle, false);
            this.SpeechSynthesizer.Speak(text);
        }
        public void SpeakAsync(string text, bool flashClient)
        {
            if (flashClient) WinAPI.FlashWindow(this.Client.TibiaProcess.MainWindowHandle, false);
            this.SpeechSynthesizer.SpeakAsync(text);
        }
        public bool IsSpeaking()
        {
            return this.SpeechSynthesizer.State == SynthesizerState.Speaking;
        }
    }
}
