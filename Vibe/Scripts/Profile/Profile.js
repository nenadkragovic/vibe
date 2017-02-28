var ProfileViewModel = function (imageUrl) {
    var self = this;

    self.image = ko.observable(imageUrl);
    self.addImage = function (el) {
        self.image(URL.createObjectURL(el));
    }

    self.followersOpen = ko.observable(false);
    self.followingOpen = ko.observable(false);

    self.openFollowers = function () {
        self.followersOpen(!self.followersOpen());
    }
    self.openFollowing = function () {
        self.followingOpen(!self.followingOpen());
    }

    self.followers = ko.observable();
    self.following = ko.observable();

    self.GetFollowers = function () {
        var url = "../../Profile/GetFollowers";
        $.ajax({
            type: "get",
            url: url,
            success: function (data) {
                self.followers(data);
            },
            error: function (data) {
                console.log("Error");
            }
        });
    }
    self.GetFollowers();
    self.GetFollowing = function () {
        var url = "../../Profile/GetFollowing";
        $.ajax({
            type: "get",
            url: url,
            success: function (data) {
                self.following(data);
            },
            error: function (data) {
                console.log("Error");
            }
        });
    }
    self.GetFollowing();

    self.SelectedProfile = ko.observable();
    self.playlists = ko.observable();

    self.SelectPLaylist = function (data, event) {

        self.SelectedProfile(data);

        console.log(self.SelectedProfile());

        fillData(self.SelectedProfile());

        $(".bg-overlay").css({ "display": "block" });
        $(".popup").css({ "display": "block" });

    }
    var fillData = function (profile) {
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
    self.unfollow = function () {
        $.ajax({
            type: "post",
            url: "../../Profile/Unfollow",
            data: { Id: self.SelectedProfile().Id },
            success: function (data) {
                alert("You are not following: " + self.SelectedProfile().Name + " anymore.");
                self.GetFollowing();
            },
            error: function (data) {
                alert("Error ocured!");
                self.GetFollowing();
            }
        });
    }
}