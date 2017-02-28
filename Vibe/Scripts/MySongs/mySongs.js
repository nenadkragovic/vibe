var Model;
(function (Model, ko) {
    function ViewModel() {
        var player;
        self.songUrl = ko.observable();
        self.songs = ko.observableArray();
        self.searchText = ko.observable();
        self.search = function () {
            var url = "../../MySongs/Search";
            $.ajax({
                type: "GET",
                data: { "query": self.searchText() },
                url: url,
                success: function (data) {
                    self.songs(data);
                },
                error: function (data) {
                    console.log(data);
                }
            });
        }

        //player
        self.song = ko.observable([{ "Name": "", "Artist": "", "Album": "", "FilePath": "", "Id": "588d291b9bd99a038c510787", "Length": "0" }]);
        self.volume = ko.observable(100);
        self.isPlaying = ko.observable(false);
        self.play = function () {
            player.play();
            self.isPlaying(true);
        }
        self.pause = function () {
            player.pause();
            self.isPlaying(false);
        }
        self.forward = function () {
            var index = self.songs().indexOf(self.song());
            if(index < self.songs().length - 1)
                self.selectSong(self.songs()[index + 1]);
        }
        self.backward = function () {
            var index = self.songs().indexOf(self.song());
            if (index > 0)
                self.selectSong(self.songs()[index - 1]);
        }
        self.muted = function () {
            return self.volume() == 0;
        }
        self.lowVolume = function () {
            return self.volume() < 50 && self.volume() > 0;
        }
        self.loudVolume = function () {
            return self.volume() >= 50;
        }
        self.times = ko.observable("");
        self.selectSong = function (el) {
            player.pause();
            self.song(el);
            self.songUrl('/Home/Audio/' + self.song().songRef);
            player.load();
            self.play();
        }
        $(document).ready(function () {
            player = document.getElementById('audioPlayer');
            $("#volumeSlider").slider({
                value: 100,
                min: 0,
                max: 100,
                step: 1,
                animate: true,
                slide: function (event, ui) {
                    self.volume(ui.value);
                    player.volume = ui.value / 100;
                },
                stop: function (event, ui) {
                    self.volume(ui.value);
                    player.volume = ui.value / 100;
                }
            });
            $("#songProgress").slider({
                value: 0,
                min: 0,
                max: 100,
                step: 1,
                animate: true,
                slide: function (event, ui) {
                    player.currentTime = Math.round(ui.value / 100 * player.duration);
                },
                stop: function (event, ui) {
                    player.currentTime = Math.round(ui.value / 100 * player.duration);
                }
            });
            var progressbar = $("#progressBar");
            var loadedBar = $("#loadedBar");
            player.ontimeupdate = function () {
                $("#progressBar").css({ "width": "" + Math.round(100 / player.duration * player.currentTime) + "%" });
                $("#loadedBar").css({ "width": "" + Math.round(100 / player.duration * player.buffered.end(player.buffered.length - 1)) + "%" });
                $("#songProgress").slider("value", Math.round(100 / player.duration * player.currentTime));
                self.times(convertSecToMin(player.currentTime) + " / " + convertSecToMin(player.duration));
            }
        });
        var convertSecToMin = function (sec) {
            var minutes = Math.floor(sec / 60);
            var seconds = sec - minutes * 60;
            return "" + Math.round(minutes) + ":" + Math.round(seconds);
        }

        //Actions
        self.addToPlaylist = function (data, event) {
            $(".bg-overlay").css({ "display": "block" });
            $(".popup").css({ "display": "block" });
            self.selectedSongId(data);
        }
        self.tumbsUp = function (data, event) {
            alert(event);
        }
        self.deleteSong = function (data, event) {
            $.ajax({
                type: "post",
                url: "../../MySongs/Delete",
                data: { songId: data.SongId },
                success: function (data) {
                    self.search();
                }
            });
        }
        self.selectedSongId = ko.observable();
        self.addSongToPlaylist = function () {
            var playlistId = $("#addToPlaylistForm option:selected").val();
            console.log(self.selectedSongId().SongId);
            $.ajax({
                type: "post",
                url: "../../Playlists/AddSongToPlaylist",
                data: { songId: self.selectedSongId().SongId, playlistId: playlistId },
                success: function (data) {
                    $(".bg-overlay").css({ "display": "none" });
                    $(".popup").css({ "display": "none" });
                    alert(self.selectedSongId().Name + " successfuly add to " + $("#addToPlaylistForm option:selected").text());
                }
            });
        }



        document.body.onkeydown = function (e) {
            if (e.keyCode == 32) {
                if (self.isPlaying())
                    self.pause();
                else self.play();
            }
            if (e.keyCode == 38) {
                $("#volumeSlider").slider("value", $("#volumeSlider").slider("value") + 5);
                if (player.volume <= 0.95)
                    player.volume += 0.05;
                else player.volume = 100;
            }
            if (e.keyCode == 40) {
                $("#volumeSlider").slider("value", $("#volumeSlider").slider("value") - 5);
                if (player.volume >= 0.05)
                    player.volume -= 0.05;
                else player.volume = 0;
            }
            if (e.keyCode == 37) {
                self.backward();
            }
            if (e.keyCode == 39) {
                self.forward();
            }

        }
        // My Songs binding 
        self.formVisible = ko.observable(false);

        self.showUpload = function () {
            if (self.formVisible()) {
                self.formVisible(false)
            }
            else {
                self.formVisible(true);
            }
        }
        self.songFile = ko.observable();
        self.songName = ko.observable();
        self.artistName = ko.observable();
        self.albumName = ko.observable();
        self.addFile = function addFile(file) {
            if (!file) {
                return;
            }
            self.songFile(file);
        };
        self.saveSong = function () {
            var url = "../../MySongs/AddSong";
            $.ajax({
                type: "POST",
                data: { "file": self.addFile(), "name": self.songName(), "artist": self.artistName(), "album": self.albumName() },
                url: url,
                success: function (data) {
                    alert("Song has successfuly added");
                },
                error: function () {
                    alert("Error! Song was not added");
                }
            });
        }
    }
    var VM = new ViewModel()
    $.extend(Model, { ViewModel: VM });
    $(document).ready(function () {
        ko.applyBindings(VM);
        self.search();
    });

})(Model, ko);