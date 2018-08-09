(function($) {

    var global = this;

    $(document).ready(function() {

        var ELAPSED_TIME_TO_DECLARED_AS_DEAD = 10000;
        var ELAPSED_TIME_TO_REMOVE = 30000;
        var BREAKER_CHECK_INTERVAL = 5000;

        var statusDescriptions = ["Open", "Half-Open", "Closed", "Forced Open", "Forced Half-Open", "Forced Closed", "Dead"];
        var statusColors = ["#900", "#f90", "#090", "#900", "#f90", "#090", "#999"];

        var sorter = {
            orderById: function(lhs, rhs) {
                return lhs.id() > rhs.id() ? 1 : -1;
            },
            orderByProcessId: function(lhs, rhs) {
                if (lhs.processId() === rhs.processId()) {
                    return sorter.orderById(lhs, rhs);
                }
                return lhs.processId() > rhs.processId() ? 1 : -1;
            },
            orderByName: function(lhs, rhs) {
                if (lhs.machineName() > rhs.machineName()) return 1;
                if (lhs.machineName() < rhs.machineName()) return -1;
                if (lhs.displayName() > rhs.displayName()) return 1;
                if (lhs.displayName() < rhs.displayName()) return -1;
                return sorter.orderByProcessId(lhs, rhs);
            },
            orderByMimic: function(lhs, rhs) {
                if (!lhs.isAcceptingCommands() && rhs.isAcceptingCommands()) return 1;
                if (lhs.isAcceptingCommands() && !rhs.isAcceptingCommands()) return -1;
                return sorter.orderByName(lhs, rhs);
            },
            orderByFailures: function(lhs, rhs) {
                var lval = lhs.failureCount();
                var rval = rhs.failureCount();
                if (lval === rval) return sorter.orderByMimic(lhs, rhs);
                return lval < rval ? 1 : -1;
            },
            orderByStatus: function(lhs, rhs) {
                var lval = lhs.status();
                var rval = rhs.status();
                lval = (lval === 6 ? 4 : lval % 3);
                rval = (rval === 6 ? 4 : rval % 3);
                if (lval === rval) return sorter.orderByMimic(lhs, rhs);
                return lval > rval ? 1 : -1;
            },
            orderByTotalAttempted: function(lhs, rhs) {
                var lval = lhs.totalAttempted();
                var rval = rhs.totalAttempted();
                if (lval === rval) return sorter.orderByMimic(lhs, rhs);
                return lval < rval ? 1 : -1;
            },
            orderByTotalFailed: function(lhs, rhs) {
                var lval = lhs.totalFailed();
                var rval = rhs.totalFailed();
                if (lval === rval) return sorter.orderByMimic(lhs, rhs);
                return lval < rval ? 1 : -1;
            },
            orderByTotalSucceeded: function(lhs, rhs) {
                var lval = lhs.totalSucceeded();
                var rval = rhs.totalSucceeded();
                if (lval === rval) return sorter.orderByMimic(lhs, rhs);
                return lval < rval ? 1 : -1;
            }
        };

        var mapObjectToArray = function(o) {
            var a = [];
            for (var key in o) {
                if (o.hasOwnProperty(key)) {
                    a.push({ "name": key, "value": o[key] });
                }
            }
            return a;
        };

        var mapArrayToObject = function(a) {
            var o = {};
            for (var i = 0; i < a.length; i++) {
                var item = a[i];
                o[item.name] = item.value;
            }
            return o;
        };

        var Breaker = function(state) {

            var self = this;

            self.id = ko.observable(state.CircuitBreakerId);
            self.processId = ko.observable(state.Pid);
            self.sequence = ko.observable(+state.RequestMessageSequenceNumber);
            self.status = ko.observable(+state.Status);
            self.machineName = ko.observable(state.MachineName);
            self.displayName = ko.observable(state.InformationalPropertyBag["circuit.displayname"]);
            self.failureCount = ko.observable(+state.InformationalPropertyBag["circuit.failure.count"]);
            self.totalAttempted = ko.observable(+state.InformationalPropertyBag["circuit.total.attempted"]);
            self.totalFailed = ko.observable(+state.InformationalPropertyBag["circuit.total.failed"]);
            self.totalSucceeded = ko.observable(+state.InformationalPropertyBag["circuit.total.succeeded"]);
            self.isAcceptingCommands = ko.observable(state.InformationalPropertyBag["circuit.isacceptingcommands"] == "true");
            self.callDurations = ko.observableArray([]);
            if (state.InformationalPropertyBag["circuit.calldurationlistinms"]) {
                self.callDurations(state.InformationalPropertyBag["circuit.calldurationlistinms"].split(","));
            }
            self.informationalProperties = ko.observableArray(mapObjectToArray(state.InformationalPropertyBag));
            self.updatableProperties = ko.observableArray(mapObjectToArray(state.UpdateablePropertyBag));

            self.statusDescription = ko.computed(function() {
                return statusDescriptions[self.status()];
            });
            self.statusColor = ko.computed(function() {
                return statusColors[self.status()];
            });
            self.statusCss = ko.computed(function() {
                var status = self.status();
                var cssClass = "circuit";
                if (status === 6) {
                    cssClass += " dead";
                } else if (status % 3 === 2) {
                    cssClass += " closed";
                } else if (status % 3 === 1) {
                    cssClass += " half-open";
                } else {
                    cssClass += " open";
                }
                if (!self.isAcceptingCommands()) {
                    cssClass += " mimic";
                }
                return cssClass;
            });

            self.openProperties = function(item) {
                var $popup = $("#propertiesPopup");
                $popup.on("show", function() {
                    ko.applyBindings(item, $popup.get(0));
                });
                $popup.modal({
                    backdrop: true,
                    show: true
                });
            };
            self.updateProperties = function() {
                var $popup = $("#propertiesPopup");
                var propertyBag = mapArrayToObject(self.updatableProperties());
                $.connection.managementHub.server.updatePropertyBag(self.id(), propertyBag);
                $popup.modal("hide");
            };

            self.openDetails = function(item) {
                var $popup = $("#detailsPopup");
                $popup.on("show", function() {
                    ko.applyBindings(item, $popup.get(0));
                });
                $popup.modal({
                    backdrop: true,
                    show: true
                });
            };

            self._lastAttempted = self.totalAttempted();
            self.attemptsPerSecond = ko.observableArray([]);
            self.calcAttemptsPerSecond = function() {
                var totalAttempted = self.totalAttempted();
                self.attemptsPerSecond.push(totalAttempted - self._lastAttempted);
                self._lastAttempted = totalAttempted;
                if (self.attemptsPerSecond().length >= 60) {
                    self.attemptsPerSecond.shift();
                }
            };

            self._lastAverageDuration = 0;
            
            self.averageCallDuration = ko.computed(function() {
                var totalDuration = 0;
                var callDurations = self.callDurations();
                var noOfCalls = callDurations.length;
                if (noOfCalls) {
                    $.each(callDurations, function() {
                        totalDuration += +this;
                    });
                    self._lastAverageDuration = Math.floor(totalDuration / noOfCalls);
                }
                return self._lastAverageDuration;
            });

            self.averageCallDurations = ko.observableArray([]);
            self.calcAverageCallDurations = function() {
                var callDurations = self.callDurations();
                $.each(callDurations, function() {
                    self.averageCallDurations.push(+this);
                    if (self.averageCallDurations().length >= 25) {
                        self.averageCallDurations.shift();
                    }
                });
            };

            self.visualise = ko.computed(function() {
                $("#" + self.id() + " .bar").sparkline(self.averageCallDurations(), {
                    type: 'bar',
                    height: '40',
                    zeroColor: "#ccc",
                    barColor: ['#666'],
                });
                $("#" + self.id() + " .line").sparkline(self.attemptsPerSecond(), {
                    type: 'line',
                    height: '40',
                    width: '120'
                });
            });

            self.visible = ko.computed(function() {
                var filterText = model.filterText();
                if (filterText) {
                    var filterPattern = new RegExp(filterText, "i");
                    if (!(filterPattern.test(self.machineName()) || filterPattern.test(self.displayName()))) {
                        return false;
                    }
                }
                return true;
            });
        };
        Breaker.prototype = {
            update: function(state) {
                var self = this;
                self.sequence(+state.RequestMessageSequenceNumber);
                self.status(+state.Status);
                self.informationalProperties(+state.InformationalPropertyBag["circuit.failure.count"]);
                self.failureCount(+state.InformationalPropertyBag["circuit.failure.count"]);
                self.totalAttempted(+state.InformationalPropertyBag["circuit.total.attempted"]);
                self.totalFailed(+state.InformationalPropertyBag["circuit.total.failed"]);
                self.totalSucceeded(+state.InformationalPropertyBag["circuit.total.succeeded"]);
                self.isAcceptingCommands(state.InformationalPropertyBag["circuit.isacceptingcommands"] == "true");
                if (state.InformationalPropertyBag["circuit.calldurationlistinms"]) {
                    self.callDurations(state.InformationalPropertyBag["circuit.calldurationlistinms"].split(","));
                } else {
                    self.callDurations([]);
                }
                self.lastReportedTime = new Date().getTime();

                self.calcAttemptsPerSecond();
                self.calcAverageCallDurations();
            },
            forceClosed: function() {
                $.connection.managementHub.server.forceClosed(this.id());
            },
            forceHalfOpen: function() {
                $.connection.managementHub.server.forceHalfOpen(this.id());
            },
            forceOpen: function() {
                $.connection.managementHub.server.forceOpen(this.id());
            }
        };

        var DashboardModel = function() {
            this.selectedSorter = sorter.orderByStatus;
            this.breakers = ko.observableArray([]);
            this.filterText = ko.observable("");
            this.isAdmin = ko.observable(true);
        };
        DashboardModel.prototype = {
            indexOf: function(state) {
                for (var i = 0, l = this.breakers().length; i < l; i++) {
                    if (this.breakers()[i].id() === state.CircuitBreakerId) {
                        return i;
                    }
                }
                return -1;
            },
            addBreaker: function(state) {
                var index = this.indexOf(state);
                if (index === -1) {
                    this.breakers.push(new Breaker(state));
                    this.sort();
                } else {
                    var breaker = this.breakers()[index];
                    breaker.update(state);
                }
            },
            removeBreakers: function(ids) {
                this.breakers.remove(function(breaker) {
                    return ids.indexOf(breaker.id()) !== -1;
                });
            },
            forceClosed: function() {
                $.each(this.breakers(), function() {
                    if (this.visible()) {
                        $.connection.managementHub.server.forceClosed(this.id());
                    }
                });
            },
            forceHalfOpen: function() {
                $.each(this.breakers(), function() {
                    if (this.visible()) {
                        $.connection.managementHub.server.forceHalfOpen(this.id());
                    }
                });
            },
            forceOpen: function() {
                $.each(this.breakers(), function() {
                    if (this.visible()) {
                        $.connection.managementHub.server.forceOpen(this.id());
                    }
                });
            },
            updateVisualisations: function(nodes, item) {
                item.visualise();
            },
            orderByName: function() {
                this.sort(sorter.orderByMimic);
            },
            orderByFailures: function() {
                this.sort(sorter.orderByFailures);
            },
            orderByStatus: function() {
                this.sort(sorter.orderByStatus);
            },
            orderByTotalAttempted: function() {
                this.sort(sorter.orderByTotalAttempted);
            },
            orderByTotalFailed: function() {
                this.sort(sorter.orderByTotalFailed);
            },
            orderByTotalSucceeded: function() {
                this.sort(sorter.orderByTotalSucceeded);
            },
            sort: function(sortBy) {
                if (sortBy) {
                    this.selectedSorter = sortBy;
                }
                this.breakers.sort(this.selectedSorter);
            },
            openLogFiles: function() {
                var $popup = $("#logFilesPopup");
                $popup.modal({
                    backdrop: true,
                    show: true
                });
            }
        };

        var model = new DashboardModel();
        ko.applyBindings(model, $("#circuits").get(0));

        $.connection.managementHub.client.updateCircuitBreakerState = function(state) {
            model.addBreaker(state);
            model.sort();
        };
        $.connection.hub.disconnected(function() { location.reload(); });
        $.connection.hub.start();

        function removeRogueBreakers() {
            $.each(model.breakers(), function() {
                var now = new Date().getTime();
                var elapsed = now - this.lastReportedTime;
                if (elapsed > ELAPSED_TIME_TO_REMOVE) {
                    model.breakers.remove(this);
                } else if (elapsed > ELAPSED_TIME_TO_DECLARED_AS_DEAD) {
                    this.status(6);
                }
            });
            global.setTimeout(removeRogueBreakers, BREAKER_CHECK_INTERVAL);
        }

        removeRogueBreakers();
    });

})(jQuery);