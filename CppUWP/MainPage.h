#pragma once

#include "MainPage.g.h"
#include "winrt/Net5BgTaskComponent.h"

namespace winrt::CppUWP::implementation
{
    struct MainPage : MainPageT<MainPage>
    {
        MainPage();

        int32_t MyProperty();
        void MyProperty(int32_t value);

        void ClickHandler(Windows::Foundation::IInspectable const& sender, Windows::UI::Xaml::RoutedEventArgs const& args);

        winrt::Net5BgTaskComponent::BgTask bgtask;
    };
}

namespace winrt::CppUWP::factory_implementation
{
    struct MainPage : MainPageT<MainPage, implementation::MainPage>
    {
    };
}
