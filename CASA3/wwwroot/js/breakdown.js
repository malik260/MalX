// Write your JavaScript code.
var app = angular.module("payApp", []);
app.controller('biviController', function ($scope, $http,$location) {

    //function biviController() {
   // alert("seen");
  //  var vm = $scope;
   // vm.GetBreakDown = getBreakDown;
   // $scope.CompanyBreakDowns = [];
    $scope.SaveBreakdown = saveBreakdown;
    $scope.DeleteBreakDown = deleteBreakDown;
    $scope.SaveNonTBreakdown = saveNonTBreakdown;
    $scope.StaffList = getStafflists;
    $scope.logout = logout;
    $scope.EmailSlip = emailSlip;

   // $scope.CompanyBreakDowns = CompanyBreakDowns;

    function getBreakDown() {
       
        var url = "/Payrolls/GetCompanyBreakdown";

        $http.get(url).then(function (response) {
            
            $scope.CompanyBreakDowns = JSON.parse(response.data.data);
           
            if ($scope.CompanyBreakDowns.length > 0) {
                
                var total = 0;
                for (var i = 0; i < $scope.CompanyBreakDowns.length; i++) {
                    total = total + $scope.CompanyBreakDowns[i].Percentage;
                }
                $scope.CurrentPercentage = total;
            }
        });
    }
    
    function getNonTBreakDown() {
        
        var url = "/Payrolls/GetNonTaxableBreakdownNon";

        $http.get(url).then(function (response) {
            
            $scope.CompanyNTBreakDowns = JSON.parse(response.data.data);
            
            if ($scope.CompanyNTBreakDowns.length > 0) {

                var total = 0;
                for (var i = 0; i < $scope.CompanyNTBreakDowns.length; i++) {
                    total = total + $scope.CompanyNTBreakDowns[i].Percentage;
                }
                $scope.CurrentNTPercentage = total;
                
            }
        });
    }
    
    function saveNonTBreakdown(name, percentage) {
       
        var url = "/Payrolls/AddNonTaxableBreakdown?name=" + name + "&percentage=" + percentage;
        if ($scope.CurrentNTPercentage + percentage > 100) {
            errorAlert("Sorry the maximum package is 100%");
            return;
        }
        
        $http.post(url).then(function (response) {
            
            infoAlert(response.data.data);
            window.location.reload();
        });
    }
    function saveBreakdown(name, percentage) {
        
        var url = "/Payrolls/AddCompanyBreakdown?name=" + name + "&percentage=" + percentage;
        if ($scope.CurrentPercentage + percentage > 100) {
            errorAlert("Sorry the maximum package is 100%");
            return;
        }
        
        $http.post(url).then(function (response) {
            
            infoAlert(response.data.data);
            window.location.reload();
        });
    }
    function deleteBreakDown(id) {
        
        var url = "/Payrolls/DeleteCompanyBreakdown?id=" + id;

        $http.post(url).then(function (response) {
            
            infoAlert(response.data.data);
            window.location.reload();
            
        });
    }

    function getStafflists() {
       
        var url = "/Payrolls/GetStafflist";

        $http.get(url).then(function (response) {
            $scope.StaffList = JSON.parse(response.data.data);
            
        });
    }

    function emailSlip(id) {
        debugger;
        var url = "/Payrolls/Sendslip?id=" + id;

        $http.get(url).then(function (response) {
            debugger;
            infoAlert(response.data.data);
            window.location.reload();
        });
    }

    function logout() {
        var url = "/Account/LogOut";
       
        $http.post(url).then(function (response) {
            
            infoAlert(response.data.data);
         
        });

    }
        
    getStafflists();
    
    getBreakDown();
   getNonTBreakDown();

})


