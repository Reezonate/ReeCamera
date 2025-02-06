using System;

namespace ReeSabers.UI;

public interface IReeModal {
    public void Resume(object state, Action closeAction);
    public void Pause();
    public void Interrupt();
}