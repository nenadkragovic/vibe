
    $(".player-back").remove();
    $("#audioPlayer").remove();
    $(document).ready(function () {
        $(".player-back").remove();
        $("#audioPlayer").remove();
    });

    var ViewModel = function () {
        var self = this;



        self.searchText = ko.observable('');
        self.users = ko.observableArray();

        self.search = function () {
            var url = "../../Profile/SearchUsers";
            $.ajax({
                type: "get",
                url: url,
                data: { "query": self.searchText() },
                success: function (data) {
                    self.users(data);
                },
                error: function (data) {
                    console.log("Error");
                }
            });
        }

        self.SelectedProfile = ko.observable();
        self.playlists = ko.observable();

        self.SelectPLaylist = function (data, event) {

            self.SelectedProfile(data);

            console.log(self.SelectedProfile());

            fillData(data);

        }
        var fillData= function(profile){
            var url = "../../Profile/GetUsersPlaylists";
            $.ajax({
                type: "get",
                url: url,
                data: { "id": profile.Id },
                success: function (data) {
                    self.playlists(data);
                },
                error: function (data) {
                    console.log("Error");
                }
            });
        }

        self.follow = function () {
            $.ajax({
                type: "post",
                url: "../../Profile/Follow",
                data: { Id: self.SelectedProfile().Id },
                success: function (data) {
                    alert("You are now following: " + self.SelectedProfile().Name);
                },
                error: function (data) {
                    alert("Error ocured!");
                }
            });
        }
    }
    var vm = new ViewModel();
    ko.applyBindings(vm);

