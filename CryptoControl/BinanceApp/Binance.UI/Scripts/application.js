var controller = undefined;
angular.module('Application', ['ui.bootstrap', "highcharts-ng"]).run(["$rootScope", "$compile", "$http", "$q", "$uibModal", function ($rootScope, $compile, $translate, $http, $q, $uibModal) { }])
  .controller('MainController', function ($scope, $rootScope, $q, $http, $uibModal) {
      controller = $scope;
      $scope.init = function () {
          $scope.callProc();
      }

      $scope.callProc = function () {
          if ($scope.selectedCoin == undefined) {
              console.log("listCalculatedOpenTransactions");
              $scope.listCalculatedOpenTransactions();
              setTimeout(function () {
                  $scope.callProc();
              }, 15000);
          } else {
              console.log("listSellTransactions");
              $scope.listSellTransactions($scope.selectedCoin);
              setTimeout(function () {
                  $scope.callProc();
              }, 15000);
          }
      }

      $scope.updateCoin = function (type,id) {
          $scope.get("Home/updateTransaction?id=1&amount=" + $scope.selectedCoin.Amount + "&value=" + $scope.selectedCoin.BuyPrice + "&symbol=" + $scope.selectedCoin.Symbol + "&index=" + $scope.selectedCoin.Index + "&type=" + type + "&status=" + $scope.selectedCoin.Status + "&old=" + $scope.selectedCoin.OldBuyTotal).then(function (data) {


              $scope.listCalculatedOpenTransactions();

          });
      }
    
      $scope.sellCoin = function (type, id) {
          $scope.get("Home/AddSellTransaction?id=1&amount=" + $scope.selectedCoin.SellAmount + "&value=" + $scope.selectedCoin.SellPrice + "&symbol=" + $scope.selectedCoin.Symbol + "&index=" + $scope.selectedCoin.Index).then(function (data) {


              $scope.listCalculatedOpenTransactions();

          });
      }

      $scope.targetCoin = function (type, id) {
          $scope.get("Home/AddTargetTransaction?id=1&amount=" + $scope.selectedCoin.SellAmount + "&value=" + $scope.selectedCoin.SellPrice + "&symbol=" + $scope.selectedCoin.Symbol + "&index=" + $scope.selectedCoin.Index).then(function (data) {


              $scope.listCalculatedOpenTransactions();

          });
      }


      $scope.detailCoin = function (coin) {
          $scope.selectedCoin = coin;
          $scope.listSellTransactions(coin);
          // $scope.getCandleSticks(coin);
      }

      $scope.back = function () {
        
              $scope.get("Home/ListAllSellTransactions").then(function (data) {
                  $scope.selectedCoin = undefined;
                  $scope.sellTransactions = data.Data.SellTransactions;
              });
          
      }
      $scope.getCandleSticks = function (coin) {
          $scope.get("Home/getCandleSticks?coin=" + coin).then(function (data) {

              $scope.candles = data.Data;
              buildCandleChart($scope.candles)
          });
      }

      $scope.listCalculatedOpenTransactions = function () {
          $scope.get("Home/ListCalculatedOpenTransactions").then(function (data) {
           
              $scope.mainData = data.Data;
              $scope.initChart();
              $scope.transactions = data.Data.Transactions;
              $scope.sellTransactions = data.Data.SellTransactions;
              $scope.targetTransactions = data.Data.TargetTransactions;
              
          });
      }

      $scope.listSellTransactions = function (coin) {
          $scope.get("Home/ListSellTransactions/"+coin.Symbol).then(function (data) {

              $scope.sellTransactions = data.Data.SellTransactions;
          });
      }

     

      $scope.get = function (url) {
          var def = $q.defer();
          $http({
              method: 'GET',
              url: url
          }).then(function successCallback(response) {
              def.resolve(response.data);
          }, function errorCallback(response) {
              def.reject(response);
          });
          return def.promise;
      }

      $scope.addModal = function () {
          var modalInstance = $uibModal.open({
              templateUrl: '/Content/add.html',
              controller: function ($scope, $uibModalInstance) {
                  $scope.get = function (url) {
                      var def = $q.defer();
                      $http({
                          method: 'GET',
                          url: url
                      }).then(function successCallback(response) {
                          def.resolve(response.data);
                      }, function errorCallback(response) {
                          def.reject(response);
                      });
                      return def.promise;
                  }
                  $scope.changeCoin = function () {
                      for (var x = 0; x < $scope.icos.length; x++) {
                          if ($scope.icos[x].Symbol == $scope.selectedCoin.Symbol)
                              $scope.selectedCoin.BuyPrice = $scope.icos[x].Price;
                      }
                  }

                  $scope.Message = "Add New Transaction";
                
                      $scope.get("Home/GetAllIcos").then(function (data) {

                          $scope.icos = data.Data;
                        
                      });
                 
                      $scope.ok = function () {
                          $scope.get("Home/AddBuyTransaction?amount=" + $scope.selectedCoin.Amount+"&value=" +$scope.selectedCoin.BuyPrice+"&symbol="+ $scope.selectedCoin.Symbol ).then(function (data) {

                              controller.listCalculatedOpenTransactions();
                              $uibModalInstance.close('closed');

                          });
                     
                  };
                  $scope.cancel = function () {
                      $uibModalInstance.dismiss('cancel');
                  };
              }
              //,size: 'sm'
          });
          modalInstance.result.then(function () {
             
              //cp.services.DeleteHolesCoordinatesFile($scope.surveyId, f.FileId, function (data) {
              //    $rootScope.showToaster({ type: "success", title: "Exclusão de Arquivo.", text: "Arquivo excluído com sucesso!" });
              //    $scope.listFilesAndLoadMap();
              //});
          }, function () {
          });
      }



      $scope.init();
     // var myVar = setInterval(function () { $scope.init(); }, 25000);

      $scope.chart1 = function (coins,series) {
         
          var chartContainer = $('<div></div>')
          var br = $('<br>')
          $('#coinsProfitContainer').append(chartContainer);
          
          $('#coinsProfitContainer').append(br);
           
          $(chartContainer).highcharts({
                  chart: {
                      type: 'column'
                  },
                  title: {
                      text: 'Group of Coin'
                  },
                  xAxis: {
                      categories: coins
                  },
                  yAxis: [{
                      min: 0,
                      title: {
                          text: 'BTC Value'
                      }
                  }],
                  legend: {
                      shadow: false
                  },
                  tooltip: {
                      shared: true
                  },
                  plotOptions: {
                      column: {
                          grouping: false,
                          shadow: false,
                          borderWidth: 0
                      }
                  },
                  series: series

              });

            
       
      }
      
      $scope.initChart = function () {
          var data = $scope.mainData;
          $('#coinsProfitContainer').empty();
          if (data == undefined)
              return;

          setTimeout(function () {
          $scope.coins = [];
          $scope.buyPrices = [];
          $scope.profitSeries = [];
          $scope.actualPrices = [];
          $scope.targetPrices = [];
          $scope.seriesDistibution = [];
          var hasRegs = true;
          for (var i = 0; i < data.Transactions.length;i++){
              $scope.coins[$scope.coins.length] = data.Transactions[i].Symbol;
              $scope.buyPrices[$scope.buyPrices.length] = data.Transactions[i].BuyTotal;
              $scope.actualPrices[$scope.actualPrices.length] = data.Transactions[i].ActualTotal;
              var targetPrice = 0;
              if ($scope.seriesDistibution[data.Transactions[i].Symbol] == undefined)
                  $scope.seriesDistibution[data.Transactions[i].Symbol] = {
                      name: data.Transactions[i].Symbol,
                      y: data.Transactions[i].BuyTotal
                  };
              else $scope.seriesDistibution[data.Transactions[i].Symbol].y += data.Transactions[i].BuyTotal;

              for (var x = 0; x < data.TargetTransactions.length; x++) {
                  if (data.Transactions[i].Symbol == data.TargetTransactions[x].Symbol && data.Transactions[i].Index == data.TargetTransactions[x].Index)
                      targetPrice = data.TargetTransactions[x].SellTotal;
              }
              $scope.targetPrices[$scope.targetPrices.length] = targetPrice;

              $scope.profitSeries[0] = {
                  name: 'Buy Price',
                  color: 'rgba(165,170,217,1)',
                  data: $scope.buyPrices,
                  pointPadding: 0.3,
                  pointPlacement: -0.2
              };
              $scope.profitSeries[1] = {
                  name: 'Actual Value',
                  color: 'rgba(126,86,134,.9)',
                  data: $scope.actualPrices,
                  pointPadding: 0.4,
                  pointPlacement: -0.2
              };
              $scope.profitSeries[2] = {
                  name: 'Target',
                  color: 'rgba(136,189,34,.9)',
                  data: $scope.targetPrices,
                  pointPadding: 0.5,
                  pointPlacement: -0.2
              };
               hasRegs = true;
              if (i % 10==0 && i>1) {
                  hasRegs = false;
                  $scope.chart1($scope.coins, $scope.profitSeries);
                  $scope.coins = [];
                  $scope.buyPrices = [];
                  $scope.actualPrices = [];
                  $scope.targetPrices = [];
               
              }
                
          }
            if(hasRegs) 
                $scope.chart1($scope.coins, $scope.profitSeries);

          
            var array_values = new Array();

            for (var key in $scope.seriesDistibution) {
              
                array_values.push($scope.seriesDistibution[key]);
            }

            array_values.push({
                name: "BTC",
                y: data.TotalBtcFree
            });

          $('#coinsDivision').highcharts({
              chart: {
                  plotBackgroundColor: null,
                  plotBorderWidth: null,
                  plotShadow: false,
                  type: 'pie'
              },
              title: {
                  text: '% Invested in each coin'
              },
              tooltip: {
                  pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
              },
              plotOptions: {
                  pie: {
                      allowPointSelect: true,
                      cursor: 'pointer',
                      dataLabels: {
                          enabled: true,
                          format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                          style: {
                              color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                          }
                      }
                  }
              },
              series: [{
                  name: 'Brands',
                  colorByPoint: true,
                  data: array_values
              }]
          });
          }, 500);
       
          //var ctx = document.getElementById("myChart");
          //var myChart = new Chart(ctx, {
          //    type: 'line',
          //    data: {
          //        labels: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
          //        datasets: [{
          //            data: [15339, 21345, 18483, 24003, 23489, 24092, 12034],
          //            lineTension: 0,
          //            backgroundColor: 'transparent',
          //            borderColor: '#007bff',
          //            borderWidth: 4,
          //            pointBackgroundColor: '#007bff'
          //        }, {
          //            data: [12339, 21315, 18483, 24003, 23489, 24092, 12034],
          //            lineTension: 0,
          //            backgroundColor: 'transparent',
          //            borderColor: '#007bff',
          //            borderWidth: 4,
          //            pointBackgroundColor: '#007bff'
          //        }]
          //    },
          //    options: {
          //        scales: {
          //            yAxes: [{
          //                ticks: {
          //                    beginAtZero: false
          //                }
          //            }]
          //        },
          //        legend: {
          //            display: false,
          //        }
          //    }
          //});
      }
     
  });



function buildCandleChart(candles) {
    //anychart.onDocumentReady(function () {
    //    var data = "";
    //    for (var i = 0 ; i < candles.length; i++) {
    //        if(i==candles.length-1)
    //            data += candles[i].OpenTime + "," + candles[i].Open + "," + candles[i].High + "," + candles[i].Low + "," + candles[i].Close + "," + candles[i].CloseTime;
    //        else
    //            data += candles[i].OpenTime + "," + candles[i].High + "," + candles[i].Low + "," + candles[i].Close + "," + candles[i].CloseTime + "\n";
    //    }
    //    // The data used in this sample can be obtained from the CDN
    //    // https://cdn.anychart.com/csv-data/csco-daily.csv
    //     // anychart.data.loadCsvFile('https://cdn.anychart.com/csv-data/csco-daily.csv', function (data) {

    //        // create data table on loaded data
    //        var dataTable = anychart.data.table();
    //        dataTable.addData(data);

    //        // map loaded data for the ohlc series
    //        var mapping = dataTable.mapAs({
    //            'open': 1,
    //            'high': 2,
    //            'low': 3,
    //            'close': 4
    //        });

    //        // map loaded data for the scroller
    //        var scrollerMapping = dataTable.mapAs();
    //        scrollerMapping.addField('value', 5);

    //        // create stock chart
    //        var chart = anychart.stock();

    //        // create first plot on the chart
    //        var plot = chart.plot(0);
    //        // set grid settings
    //        plot.yGrid(true)
    //          .xGrid(true)
    //          .yMinorGrid(true)
    //          .xMinorGrid(true);

    //        // create EMA indicators with period 50
    //        plot.ema(dataTable.mapAs({
    //            'value': 4
    //        })).series().stroke('1.5 #455a64');

    //        var series = plot.candlestick(mapping);
    //        series.name('CSCO');
    //        series.legendItem().iconType('rising-falling');

    //        // create scroller series with mapped data
    //        chart.scroller().candlestick(mapping);

    //        // set chart selected date/time range
    //        chart.selectRange('2007-01-03', '2007-05-20');

    //        // set container id for the chart
    //        chart.container('container');
    //        // initiate chart drawing
    //        chart.draw();

    //        // create range picker
    //        rangePicker = anychart.ui.rangePicker();
    //        // init range picker
    //        rangePicker.render(chart);

    //        // create range selector
    //        rangeSelector = anychart.ui.rangeSelector();
    //        // init range selector
    //        rangeSelector.render(chart);
    //    });
    //});
}