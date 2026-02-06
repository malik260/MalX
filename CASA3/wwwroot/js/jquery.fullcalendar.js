
var filtered = false;
!function ($) {
    "use strict";

    var CalendarApp = function () {
        this.$body = $("body")
        this.$modal = $('#event-modal'),
            this.$editModal = $('#edit_event'),
            this.$event = ('#external-events div.external-event'),
            this.$calendar = $('#calendar'),
            this.$saveCategoryBtn = $('.save-category'),
            this.$categoryForm = $('#add-category form'),
            this.$extEvents = $('#external-events'),
            this.$calendarObj = null
    };

    $(window).focus();
    //used to track whether the user is holding the control key

    function setEventsCopyable(isCopyable) {

        $("#calendar").fullCalendar("option", "eventStartEditable", !isCopyable);
        $(".fc-event").draggable("option", "disabled", !isCopyable);
    };

    function setRealDate(startDate) {
        var offset = new Date().getTimezoneOffset();
        var myNewDate = moment(startDate);
        myNewDate = myNewDate.add(offset, 'minutes');
        return myNewDate;
    };

    function addEmployeeSelect(selectlist) {
        $("#sortEmployee").empty();
        $("#sortEmployee").append('<option value="' + 0 + '">---Select---</option>');
        $.each(selectlist, function (i, select) {
            $("#sortEmployee").append('<option value="' + select.id + '">' + select.name + '</option>');

        })
    };

    function addClientSelect(selectlist) {
        $("#clientSort").empty();
        $("#clientSort").append('<option value="' + 0 + '">---Select---</option>');
        $.each(selectlist, function (i, select) {
            $("#clientSort").append('<option value="' + select.id + '">' + select.name + '</option>');

        })
    };

   
   

    /* on drop */
    //CalendarApp.prototype.onDrop = function (eventObj, date, start) {
    //    debugger;
    //    var $this = this;
    //    // retrieve the dropped element's stored Event Object
    //    var originalEventObject = eventObj.data('eventObject');
    //    var $categoryClass = eventObj.attr('data-class');
    //    // we need to copy it, so that multiple events don't have a reference to the same object
    //    var copiedEventObject = $.extend({}, originalEventObject);
    //    // assign it the date that was reported
    //    copiedEventObject.start = date;
    //    if ($categoryClass)
    //        copiedEventObject['className'] = [$categoryClass];
    //    // render the event on the calendar
    //    $this.$calendar.fullCalendar('renderEvent', copiedEventObject, true);
    //    // is the "remove after drop" checkbox checked?
    //    if ($('#drop-remove').is(':checked')) {
    //        // if so, remove the element from the "Draggable Events" list
    //        eventObj.remove();
    //    }
    //},
    CalendarApp.prototype.eventDrop = function (date, jsEvent, ui, resourceId) {

        var $this = this;
        setEventsCopyable(true);
        //$this.$calendar.fullCalendar("option", "eventStartEditable", false);
        var scheduleMonth = $("#month").val();
        var uri = location.href;

        const droppedEvent = date;
        const origStartDate = setRealDate(date.start._d)._d;
        const origEndDate = date.end._d;
        debugger;
        //Check if the event was dropped on another month
        var drppedTitle = droppedEvent.title.trim();
        if (drppedTitle == "") {
            newErrorAlert("You can't copy an event that have been dropped from a staff", uri);
        } else {


            origStartDate.getDate();
            // Set the start date to the new date with the original time
            let startDate = moment(origStartDate);
            origStartDate.setHours(
                origStartDate.getHours() /*- 1*/,
            );

            const endDate = origEndDate;

            // If the orginal dates were on different days we have to calculate the new end date
            //if (origStartDate.getDay() != origEndDate.getDay()) {
            //    endDate.add(
            //        droppedEvent.end._d.diff(
            //            droppedEvent.start._d,
            //            "d"
            //        ),
            //        "d"
            //    );
            //}

            origEndDate.setHours(
                origEndDate.getHours() /*- 1*/,
            );

            var myYea = origEndDate.getFullYear();
            var myMont = origEndDate.getMonth() + 1;
            var myDatee = origEndDate.getDate();
            let myDat = new Date(myYea + "-" + myMont + "-" + myDatee);
            let evDate = myDat;
            var uri;
            $.ajax({
                type: "GET",
                url: "/Rota/CopySchedule",
                // dataType: 'json',
                data: {
                    Id: date.id,
                    //StartTime: origStartDate.toUTCString(),
                    //EndTime: origEndDate.toUTCString(),
                    //Name: droppedEvent.title.trim(),
                    //Category: "bg-info",
                    //EventDate: evDate.toUTCString(),
                    Day: origStartDate.getDate(),
                   // BreakPeriod: 0,
                },
                success: function (result) {
                    debugger;
                    if (!result.isError) {
                        var getCopyResult = result.data;

                        if (getCopyResult.title !== null) {
                            $this.$calendarObj.fullCalendar('renderEvent', {
                                id: getCopyResult.myDate,
                                title: getCopyResult.title,
                                start: getCopyResult.start,
                                end: getCopyResult.end,
                                allDay: false,
                                className: getCopyResult.className
                            }, true);
                            $this.$calendarObj.fullCalendar('updateEvent', getCopyResult);
                            window.location.reload();
                        }
                    } else if (result.isError && result.msg.includes("clashes")) {
                        uri = location.href;
                        newErrorAlert(result.msg, uri);

                    }
                    else {
                        uri = location.href;
                        newErrorAlert(result.msg, uri);

                    }
                },
                error: function (ex) {
                    errorAlert("Sorry, an error occoured. Try again");
                }
            });


            return true;   
        }


            
    }


    CalendarApp.prototype.eventAfterAllRender = function (event, element, view) {
        // make all events copyable using jQuery UI draggable
    
            $(".fc-event").each(function () {
                const $event = $(this);

                // store data so it can be copied in the drop function
                const event = $event.data("fcSeg").footprint.eventDef;
                $event.data("eventObj", event);

                // make the event draggable using jQuery UI
                $event.draggable({
                    disabled: true,
                    helper: "clone",
                    revert: true,
                    revertDuration: 0,
                    zIndex: 999,
                    stop(event, ui) {
                        // when dragging of a copied event stops we must set them
                        // copyable again if the control key is still held down
                        setEventsCopyable(true);
                        
                    }
                });
            });
    }
    /* on click on event */
    var checkStaff = $("#isStaff").val();


    CalendarApp.prototype.onEventClick = function (calEvent, jsEvent, view) {
        var clickable = false;
        var currentUser = $("#loggedInUser").val();
        if (checkStaff == "True") {
            clickable = true;
        } else if (checkStaff != "True" && calEvent.title.includes(currentUser)) {
            clickable = true;
        }

        if (clickable) {
            debugger;
            var $this = this;
            var clientId = $('#clientId').val();

            /*Making ajax call to the db*/
            var form;
            $.ajax({
                type: 'GET',
                url: "/Shift/EditEvent", //we are calling json method
                data: { eventId: calEvent.id, clientId: clientId },
                success: function (data) {
                    //form = $("<form class='editEvent-form'></form>");
                    //form.append()
                    $("#editModal").html(data);
                    var status = $('#statusId').val();

                    if (checkStaff != "True") {
                        $("#eventStaff,#editstartTime,#editendTime").attr("disabled", true);
                        $("#reassign,#delete,#dropped,#duplicateDiv").addClass("d-none");
                        $("#validNote").hide();
                    } else if (status == "Rejected") {

                        $("#cancel,#dropped").addClass("d-none");
                        $("#validNote").hide();
                    } else if (status == "Dropped") {

                        $("#cancel,#dropped").addClass("d-none");
                        $("#validNote").hide();
                    } else {
                        $("#cancel,#reassign").addClass("d-none");
                        $("#validNote").hide();
                    }
                }
            });
            $("#edit_event").modal();




            //var form = $("<form class='event-form'></form>");
            //form.append("<label>Remove Event</label>");
            //form.append("<div class='input-group'><input class='form-control' type=text value='" + calEvent.title + "' readonly/><span class='input-group-append'><button type='submit' class='btn btn-success btn-md'>Save</button></span></div>");
            $this.$editModal.modal({
                backdrop: 'static'
            });
            //$this.$modal.find('.delete-event').show().end().find('.save-event').hide().end().find('.modal-body').empty().prepend(form).end().find('.delete-event').unbind('click').click(function () {

            //$('#deleteBtn').click(function () {

            //    $this.$calendarObj.fullCalendar('removeEvents', function (ev) {

            //        if (ev._id == calEvent._id) {
            //            var result = deleteSchedule(calEvent.id);

            //            if (result == true) {
            //                alert("Event removed successfuly");
            //            } else if (result == "undefined") {
            //                alert("Event removed successfuly");
            //            }
            //            else {
            //                //alert("Event could not be removed");
            //                //return (ev._id == calEvent._id);
            //            }
            //        }
            //        return (ev._id == calEvent._id);

            //    });
            //    $this.$modal.modal('hide');
            //});
            $this.$editModal.find('form').on('submit', function () {
                calEvent.title = form.find("input[type=text]").val();
                $this.$calendarObj.fullCalendar('updateEvent', calEvent);
                $this.$modal.modal('hide');
                return false;
            });
        }

    }


    
   
    /* on select */
   // var monthView = false;
    CalendarApp.prototype.onSelect = function (start, end, allDay) {
        if (allClient != "True") {
            var $this = this;

            var myNewDate = setRealDate(start._d);

            $this.$modal.modal({
                backdrop: 'static'
            });
            var staffList = {};
            var getResult = {};
            var clientId = $('#clientId').val();
            var rMonth = myNewDate._d.getMonth() + 1;
            var isdate = myNewDate._d.getFullYear() + "/" + rMonth + "/" + myNewDate._d.getDate();
            var myDate = myNewDate._d.getDate();
            debugger;
            $.ajax({
                url: "/Rota/GetClientStaff",
                dataType: 'json',
                data: { ClientId: clientId },
                success: function (data) {
                    debugger;
                    if (data != null) {
                        staffList = data;

                        $("#titleName").append('<option value="' + 0 + '">---Select---</option>');
                        $("#titleName").attr('required', '');
                        $.each(staffList, function (i, staff) {
                            $("#titleName").append('<option value="' + staff.id + '">' + staff.name + '</option>');
                            $("#titleName").attr('required', '');
                        })
                    }
                },
            });

            var form = $("<form></form>");
            form.append("<div class='row'></div>");
            form.find(".row")
                /*Edit*/

                .append("<div class='col-md-12'><div class='form-group'><label class='control-label'> Staff</label><select class='select form-control'id='titleName' name='title'></select></div></div>")
                .append("<div class='col-md-6'><div class='form-group'><label class='control-label'>Start Time</label><input class='form-control' id='start' type='time' name='start'/></div></div>")
                .append("<div class='col-md-6'><div class='form-group'><label class='control-label'>End Time</label><input class='form-control' id='end' type='time' name='end'/></div></div>")

            //.append("<div class='col-md-12'><div class='form-group'><label class='control-label'>Select Class</label><select class='select form-control'id='cart' name='category'></select></div></div>")
            //.find("select[name='category']")
            //.append("<option value='bg-danger'>Danger</option>")
            //.append("<option value='bg-success'>Success</option>")
            //.append("<option value='bg-purple'>Purple</option>")
            //.append("<option value='bg-primary'>Primary</option>")
            //.append("<option value='bg-pink'>Pink</option>")
            //.append("<option value='bg-info'>Info</option>")
            //.append("<option value='bg-inverse'>Inverse</option>")
            //.append("<option value='bg-orange'>Orange</option>")
            //.append("<option value='bg-brown'>Brown</option>")
            //.append("<option value='bg-teal'>Teal</option>")
            //.append("<option value='bg-warning'>Warning</option></div></div>");
            $this.$modal.find('.delete-event').hide().end().find('.save-event').show().end().find('.modal-body').empty().prepend(form).end().find('.save-event').unbind('click').click(function () {
                form.submit();
            });

            /* gets the full DateTime span */
            function getfullDate(time, date) {
                var newTime = time.split(":");

                var completeDate = new Date(date);
                var hour = parseInt(newTime[0]);
                completeDate.setHours(hour);
                completeDate.setMinutes(newTime[1]);
                return completeDate.toString();
            };

            var newStart;
            var newEend;
            function createEvent(fullStart, fullend, isdate, myDate) {
                debugger;
                $.ajax({
                    type: "GET",
                    url: "/Rota/CreateEvents",
                    // dataType: 'json',
                    data: {
                        StartTime: fullStart,
                        EndTime: fullend,
                        Name: $('#titleName option:checked').html(),
                        ClientScheduleId: $('#clientSchedule').val(),
                        Category: "bg-info",
                        EmployeeId: $("#titleName").val(),
                        EventDate: isdate,
                        Day: myDate,
                        BreakPeriod: 0,
                        start: newStart,
                        end: newEend,
                    },
                    success: function (result) {
                        debugger;

                        getResult = result;
                        var uri = window.location.href;
                        if (getResult.msg.includes("successfully")) {
                            $this.$calendarObj.fullCalendar('renderEvent', {
                                id: getResult.data.myDate,
                                title: getResult.data.title,
                                start: getResult.data.start,
                                end: getResult.data.end,
                                allDay: false,
                                className: getResult.data.className
                            }, true);
                            location.reload();
                        } else if (getResult.msg.includes("clashes")) {

                            newErrorAlert(result.msg, uri);
                        }
                        else {
                            newErrorAlert(result.msg, uri);
                        }

                    },
                    error: function (ex) {
                        errorAlert("Sorry, an error occoured. Try again");
                    }
                });
            }

            $this.$modal.find('form').on('submit', function () {

                var start = form.find("input[name='start']").val();
                var beginning = form.find("input[name='beginning']").val();
                var end = form.find("input[name='end']").val();
                var title = form.find("select[name='title'] option:checked").html();
                // var categoryClass = form.find("select[name='category'] option:checked").val();
                newEend = end;
                newStart = start;
                var fullStart = getfullDate(start, isdate);
                var fullend = getfullDate(end, isdate);
                //making ajax call to save our event in the db
                createEvent(fullStart, fullend, isdate, myDate);

                $this.$modal.modal('hide');
                $this.$calendarObj.fullCalendar('unselect');
                return false;

            });
        }
       
    },
      
   
    CalendarApp.prototype.enableDrag = function () {
        //init events
        $(this.$event).each(function () {
            // create an Event Object (http://arshaw.com/fullcalendar/docs/event_data/Event_Object/)
            // it doesn't need to have a start or end
            var eventObject = {
                title: $.trim($(this).text()) // use the element's text as the event title
            };
            // store the Event Object in the DOM element so we can get to it later

           
            $(this).data('eventObject', eventObject);
            debugger;
            // make the event draggable using jQuery UI
            $(this).draggable({
                zIndex: 999,
                revert: true,      // will cause the event to go back to its
                revertDuration: 0  //  original position after the drag
            });



        });
    }


    var allClient = $("#allClients").val();
    /* Initializing */
    CalendarApp.prototype.init = function () {
     
        //var isStaff = $("#isStaff").val();
        var currentUser = $("#loggedInUser").val();
            this.enableDrag();
        
        /*  Initialize the calendar  */
        var date = new Date();
   
        /*Getting the already scheduled data from the database */

        var $this = this;
        var  defaultEvents = {};
        var isStaff = $("#isStaff").val();
       
        var clientScheduleId = $("#clientSchedule").val();
        var scheduleMonthId = $("#ScheduleMonthId").val();

        var selectedEmployee = $("#sortEmployee").val(); //$("#filterInput").val();
        var selectedClient = $("#clientSort").val();  //$("#filterInput").attr("name");
        // var date0 = year + "/" + month + "/" + 04;

        var drag = false;
        if (isStaff == "True") {
            drag = true;
        }

        var limit = 0;
        var uri;
        var data = {};
       
        $(document).ready(function () {
            if (allClient == "True") {
                uri = "/Rota/GetMonthlyEvent";
                data.scheduleMonthId = scheduleMonthId;
                data.employeeId = selectedEmployee ;
                data.clientId = selectedClient;
                data.isFilter = filtered;
                limit = 2;
            } else {
                uri = "/Rota/GetSchedules";
                data.clientScheduleId = clientScheduleId, data.isStaff = isStaff;
                limit = 0;
            }

            debugger;

            $(".comment .fc-content .fc-title i").remove().end();
            $.ajax({
                type: "GET",
                url: uri ,
                dataType: 'json',
                data: data,
                success: function (result) {
                    debugger;
                    if (!result.isError) {
                        
                        defaultEvents = result.data;
                        filtered = result.isFilter;
                       
                        var month = $("#month").val(); var year = $("#year").val();
                        var user = $("#clientName").val();

                        $this.$calendarObj = $this.$calendar.fullCalendar({
                            
                            showNonCurrentDates: false,
                            displayEventEnd: true,
                            defaultDate: new Date(year + '-' + month), // setting the default load month
                            //slotDuration: '00:15:00', /* If we want to split day time each 15minutes */
                            minTime: '00:00:00',
                            maxTime: '23:59:00',
                            defaultView: 'month',
                            timeFormat: 'h:mm t',
                            handleWindowResize: true,
                            height: $(window).height() - 200,
                            header: {
                                left: 'prev,next today',
                                center: 'title',
                                right: 'month,agendaWeek,agendaDay'
                            },
                            events: defaultEvents,
                            editable: drag,
                            droppable: true, // this allows things to be dropped onto the calendar !!!
                            eventLimit: limit, // allow "more" link when too many events
                            eventLimitText: "shifts",        
                            selectable: true,
                            //eventDragStop: function (event, jsEvent, ui, view) { $this.dragStop(event, jsEvent, ui, view); },
                            eventDrop: function (date, jsEvent, ui, resourceId) { $this.eventDrop(date, jsEvent, ui, resourceId); },
                            select: function (start, end, allDay) { $this.onSelect(start, end, allDay); },
                            eventClick:  function (calEvent, jsEvent, view) { $this.onEventClick(calEvent, jsEvent, view); },
                            eventAfterAllRender: function (event, element, view) { $this.eventAfterAllRender(event, element, view);  },
                          
                        });
                        if (filtered == true) {
                            $('#calendar').fullCalendar('addEventSource', defaultEvents);

                            if (selectedEmployee != null && selectedEmployee != "0") {
                                $("#sortEmployee").val(selectedEmployee); //$("#filterInput").val();

                            } else if (selectedClient != null && selectedClient != "0") {
                                $("#clientSort").val(selectedClient);
                            }
                        } else {
                            addEmployeeSelect(result.employee);
                            addClientSelect(result.client)
                        }
                       // $this.$calendarObj.fullCalendar('updateEvent', defaultEvents);
                       // $.fullCalendar('rerenderEvents');

                        $(".comment .fc-content .fc-title i").remove().end();

                        $(".comment .fc-content .fc-title ").append(" <i class='fa fa-comment'></i>");

                        if (allClient == "True") {
                            $(".fc-left").html("<button type='button' class='fc-month-button fc-button fc-state-default fc-corner-left fc-corner-right fc-state-active'>All Clients</button >")
                        } else { $(".fc-left").html("<button type='button' class='fc-month-button fc-button fc-state-default fc-corner-left fc-corner-right fc-state-active'>" + user + "</button >")}

                       


                    }
                },
            });
        });
       


 
        //on new event
        this.$saveCategoryBtn.on('click', function(){
            var categoryName = $this.$categoryForm.find("input[name='category-name']").val();
            var categoryColor = $this.$categoryForm.find("select[name='category-color']").val();
            if (categoryName !== null && categoryName.length != 0) {
                $this.$extEvents.append('<div class="external-event bg-' + categoryColor + '" data-class="bg-' + categoryColor + '" style="position: relative;"><i class="mdi mdi-checkbox-blank-circle m-r-10 vertical-middle"></i>' + categoryName + '</div>')
                $this.enableDrag();
            }

        });
    },

   //init CalendarApp
    $.CalendarApp = new CalendarApp, $.CalendarApp.Constructor = CalendarApp
    
}(window.jQuery),

    
    $("#clientSort,#sortEmployee").change(function () {
       
        $('#calendar').fullCalendar('removeEvents');
        filtered = true;
        $.CalendarApp.init();
    }),


//initializing CalendarApp
function($) {
    
    "use strict";
    $.CalendarApp.init()
    
}(window.jQuery);
