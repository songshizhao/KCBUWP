using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace EasyNote.Helper
{

    //public class ToastMessage
    //{

    //    public static void Toast(string msg)
    //    {
    //        //1. create element
    //        ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
    //        XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
    //        //2.设置消息文本
    //        XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
    //        toastTextElements[0].AppendChild(toastXml.CreateTextNode(msg));
    //        //3. 图标
    //        XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
    //        ((XmlElement)toastImageAttributes[0]).SetAttribute("src", $"ms-appx:///image/bg2.jpg");
    //        ((XmlElement)toastImageAttributes[0]).SetAttribute("alt", "logo");
    //        // 4. duration
    //        IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
    //        ((XmlElement)toastNode).SetAttribute("duration", "short");

    //        // 5. audio
    //        XmlElement audio = toastXml.CreateElement("audio");
    //        audio.SetAttribute("src", $"ms-winsoundevent:Notification.SMS");
    //        toastNode.AppendChild(audio);

    //        ToastNotification toast = new ToastNotification(toastXml);
    //        ToastNotificationManager.CreateToastNotifier().Show(toast);
    //    }















    //}








    //[ContractVersion(typeof(UniversalApiContract), 65536)]
    //public enum ToastTemplateType
    //{
    //    //
    //    // Summary:
    //    //     在三行文本中被包装的大型图像和单个字符串。
    //    ToastImageAndText01 = 0,
    //    //
    //    // Summary:
    //    //     大图像、加粗文本的一个字符串在第一行、常规文本的一个字符串包装在第二、三行中。
    //    ToastImageAndText02 = 1,
    //    //
    //    // Summary:
    //    //     大图像、加粗文本的一个字符串被包装在开头两行中、常规文本的一个字符串包装在第三行中。
    //    ToastImageAndText03 = 2,
    //    //
    //    // Summary:
    //    //     大图像、加粗文本的一个字符串在第一行、常规文本的一个字符串在第二行中、常规文本的一个字符串在第三行中。
    //    ToastImageAndText04 = 3,
    //    //
    //    // Summary:
    //    //     包装在三行文本中的单个字符串。
    //    ToastText01 = 4,
    //    //
    //    // Summary:
    //    //     第一行中加粗文本的一个字符串、覆盖第二行和第三行的常规文本的一个字符串。
    //    ToastText02 = 5,
    //    //
    //    // Summary:
    //    //     覆盖第一行和第二行的加粗文本的一个字符串。第三行中常规文本的一个字符串。
    //    ToastText03 = 6,
    //    //
    //    // Summary:
    //    //     第一行中加粗文本的一个字符串、第二行中常规文本的一个字符串、第三行中常规文本的一个字符串。
    //    ToastText04 = 7
    //}
}
