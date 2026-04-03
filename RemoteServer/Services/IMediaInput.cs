namespace RemoteServer.Services;

public interface IMediaInput
{
    void Next();
    void Previous();
    void PlayPause();
    void Stop();
    void Mute();
    void VolumeUp();
    void VolumeDown();
}