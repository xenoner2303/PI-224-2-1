using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using Presentation;
using Microsoft.Extensions.DependencyInjection;
using DTOsLibrary;
using UI.ApiClients;

namespace OnlineAuction
{
    public partial class AdminWindow : Window
    {
        private readonly IServiceProvider serviceProvider;
        private readonly AdminApiClient adminApiClient;
        private List<AuctionLotDto> _lots;
        private List<BaseUserDto> _users;
        private List<SecretCodeRealizatorDto> _secretCodes;

        public AdminWindow(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            //_lots = client. і всі поля так далі
            InitializeComponent();
        }

        private void btnLots_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCategories_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSecretCodes_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dgMainData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
