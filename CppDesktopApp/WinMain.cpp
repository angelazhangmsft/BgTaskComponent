#include "pch.h"
#include <winrt/Windows.ApplicationModel.Background.h>

using namespace winrt;
using namespace Windows::Foundation;

int __stdcall wWinMain(HINSTANCE, HINSTANCE, LPWSTR, int)
{
    init_apartment(apartment_type::single_threaded);
    Uri uri(L"http://aka.ms/cppwinrt");
    ::MessageBoxW(::GetDesktopWindow(), uri.AbsoluteUri().c_str(), L"C++/WinRT Desktop Application", MB_OK);

    std::wstring exampleTaskName{ L"BgTask" };

    auto allTasks{ Windows::ApplicationModel::Background::BackgroundTaskRegistration::AllTasks() };

    bool taskRegistered{ false };
    for (auto const& task : allTasks)
    {
        if (task.Value().Name() == exampleTaskName)
        {
            taskRegistered = true;
            break;
        }
    }

    if (!taskRegistered)
    {
        Windows::ApplicationModel::Background::BackgroundTaskBuilder builder;
        builder.Name(exampleTaskName);
        builder.TaskEntryPoint(L"Net5BgTaskComponent.BgTask");
        builder.SetTrigger(Windows::ApplicationModel::Background::SystemTrigger{
            Windows::ApplicationModel::Background::SystemTriggerType::TimeZoneChange, false });
        Windows::ApplicationModel::Background::BackgroundTaskRegistration task{ builder.Register() };
    }
}
