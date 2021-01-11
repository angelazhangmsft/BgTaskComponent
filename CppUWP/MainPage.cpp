#include "pch.h"
#include "MainPage.h"
#include "MainPage.g.cpp"

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::CppUWP::implementation
{
    MainPage::MainPage()
    {
        InitializeComponent();
        
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

}
