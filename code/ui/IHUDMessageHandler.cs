using QnClient.code.message;

namespace QnClient.code.ui;

public interface IHUDMessageHandler
{

    void UpdateKungFuBookView(KungFuBookMessage message);


    void DisplayText(string text);


    void UpdateInventoryView(InventoryMessage message);

}