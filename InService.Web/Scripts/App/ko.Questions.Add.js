function Item(sr, isc) {
    var self = this;
    self.Ans = sr;
    self.IS = isc;
}

function MyModel(itemArr) {
    var self = this;
    self.Items = ko.observableArray();
    self.Answer = ko.observable();
    self.IsCorrect = ko.observable();

    self.Total = ko.computed(function () {
        return self.Items().length;
    });

    self.addItem = function () {
        for (var i = 0; i < self.Items().length; i++) {
            if (self.Items()[i].Answer == self.Answer()) {
                alert("Answer already added!");
                return false;
            }
        }
        self.Items.push(new Item(self.Answer(), self.IsCorrect()));
        self.Answer("");
        return false;
    };

    self.removeItem = function (item) {
        self.Items.remove(item);
        return false;
    };

    self.canAddItem = ko.computed(function () {
        if (self.Answer() != null) return true;
        else return false;
    });

    self.canSubmit = ko.computed(function () {
        if (self.Items().length <= 0) {
            return false;
        }
    });

    if (itemArr != null) {
        $.map(itemArr, function (item, i) {
            self.Items.push(new Item(item.Answer));
        });
    }
}