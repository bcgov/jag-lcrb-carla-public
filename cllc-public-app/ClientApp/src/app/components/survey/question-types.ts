
function fixCheckboxes(Survey) {
  const widget = {
    name: "fixchecks",
    isFit: function(question) {
      const t = question.getType();
      return t === "radiogroup" || t === "checkbox" || t === "matrix" || t === "boolean";
    },
    isDefaultRender: true,
    afterRender: function(question, el) { // if(1) return;
      const elts = el.getElementsByTagName("input");
      for (let idx = 0; idx < elts.length; idx++) {
        const input = elts[idx];
        if (input.type !== "radio" && input.type !== "checkbox")
          continue;
        const newInput = document.createElement("input");
        for (let k of input.getAttributeNames()) {
          newInput.setAttribute(k, input.getAttribute(k));
        }
        if (! newInput.id) {
          newInput.id = (newInput.name || question.name) + "-" + idx;
        }
        newInput.checked = input.checked;
        const outer = input.parentNode;
        const contain = outer.parentNode;
        let label = undefined;
        for (let child of outer.children) {
          if (child.tagName.toLowerCase() === "span") {
            if (child.className.indexOf("circle") < 0 &&
              child.className.indexOf("check") < 0 &&
              child.className.indexOf("checkbox-material") < 0) {
              label = child;
              break;
            }
          }
        }
        if (question.getType() !== "boolean" && label)
          label = label.children[0];
        let wrap = contain;
        if (wrap.tagName.toLowerCase() !== "div") {
          wrap = document.createElement("div");
          if (question.getType() !== "boolean")
            wrap.className = newInput.type;
          contain.insertBefore(wrap, outer);
          wrap.appendChild(outer);
        }
        wrap.insertBefore(newInput, outer);
        const newLabel = document.createElement("label");
        newLabel.setAttribute("for", newInput.id);
        if (label) {
          label.style.marginLeft = "0.3em";
          newLabel.appendChild(label);
        }
        wrap.insertBefore(newLabel, outer);
        wrap.removeChild(outer);

        newInput.addEventListener("click",
          (event) => {
            const target = (event.target as HTMLInputElement);
            if (question.getType() === "matrix") {
              if (target.checked) {
                question.generatedVisibleRows.forEach(function(row, index, rows) {
                  if (row.fullName === target.name) {
                    row.value = target.value;
                  }
                });
              }
            } else if (question.getType() === "checkbox") {
              const oldValue = question.value || [];
              const index = oldValue.indexOf(target.value);
              if (index >= 0) {
                if (! target.checked) {
                  oldValue.splice(index, 1);
                  question.value = oldValue;
                }
              } else if (target.checked) {
                question.value = oldValue.concat([target.value]);
              }
            } else if (target.checked) {
              question.value = target.value;
            }
          });
      }

      question.valueChangedCallback = function() {
        if (question.getType() !== "matrix") {
          let values = question.value || [];
          if (! Array.isArray(values)) {
            values = [values];
          }
          const elts = el.getElementsByTagName("input");
          for (let i = 0; i < elts.length; i++) {
            elts[i].checked = values.indexOf(elts[i].value) >= 0;
          }
        } else {
          question.generatedVisibleRows.forEach(function(row, index, rows) {
            if (row.value) {
              const elts = el.getElementsByTagName("input");
              for (let i = 0; i < elts.length; i++) {
                if (elts[i].name === row.fullName && elts[i].value === row.value) {
                  elts[i].checked = true;
                }
              }
            }
          });
        }
      };

    },
    willUnmount: function(question, el) {}
  };

  Survey.CustomWidgetCollection.Instance.addCustomWidget(widget, "type");
}


function initHelpText(Survey) {
  const widget = {
    name: "helptext",
    title: "Help Text",
    iconName: "icon-panel",
    widgetIsLoaded: function() {
      return true;
    },
    isFit: function(question) {
      return question.getType() === "helptext";
    },
    activatedByChanged: function(activatedBy) {
      Survey.JsonObject.metaData.addClass("helptext", [], null, "empty");
      Survey.JsonObject.metaData.addProperties("helptext",
        [
          {
            name: "body:text",
          }
        ]);
    },
    htmlTemplate: "<div></div>",
    afterRender: function(question, el) {
      while (el.childNodes.length)
        el.removeChild(el.childNodes[0]);

      const outer = document.createElement("div");
      const outerCls = "panel panel-default survey-helptext";
      outer.className = outerCls;
      const header = document.createElement("div");
      header.className = "panel-heading";
      const lbl = document.createElement("label");
      lbl.className = "panel-title";
      const chk = document.createElement("input");
      chk.type = "checkbox";
      chk.checked = !! question.value;
      chk.style.visibility = "hidden";
      chk.style.width = "0px";
      chk.addEventListener("click",
        (event) => {
          const target = (event.target as HTMLInputElement);
          question.value = target.checked;
        });
      const icon = document.createElement("span");
      icon.className = "heading-icon fa fa-question-circle";
      const title = document.createElement("span");
      title.className = "title-text";
      title.appendChild(document.createTextNode(question.title));
      const expander = document.createElement("span");
      expander.className = "heading-expand fa fa-chevron-down";
      lbl.appendChild(chk);
      lbl.appendChild(icon);
      lbl.appendChild(title);
      lbl.appendChild(expander);
      header.appendChild(lbl);
      outer.appendChild(header);
      const body = document.createElement("div");
      body.className = "panel-body";
      body.appendChild(document.createTextNode(question.body || ""));
      outer.appendChild(body);
      el.appendChild(outer);

      question.valueChangedCallback = function() {
        outer.className = outerCls + (question.value ? " expanded" : "");
        expander.className = `heading-expand ${question.value ? "fa fa-chevron-up" : "fa fa-chevron-down"}`;
      };
      question.valueChangedCallback();
    },
    willUnmount: function(question, el) {}
  };

  Survey.CustomWidgetCollection.Instance.addCustomWidget(widget, "type");
}


function initInfoText(Survey) {
  const widget = {
    name: "infotext",
    title: "Info Text",
    iconName: "icon-panel",
    widgetIsLoaded: function() {
      return true;
    },
    isFit: function(question) {
      return question.getType() === "infotext";
    },
    activatedByChanged: function(activatedBy) {
      Survey.JsonObject.metaData.addClass("infotext", [], null, "empty");
      Survey.JsonObject.metaData.addProperties("infotext",
        [
          {
            name: "body:text",
          }
        ]);
    },
    htmlTemplate: "<div></div>",
    afterRender: function(question, el) {
      while (el.childNodes.length)
        el.removeChild(el.childNodes[0]);

      const outer = document.createElement("div");
      const outerCls = "panel panel-default survey-infotext expanded";
      outer.className = outerCls;
      const header = document.createElement("div");
      header.className = "panel-heading";
      const lbl = document.createElement("label");
      lbl.className = "panel-title";
      const icon = document.createElement("span");
      icon.className = "heading-icon fa fa-info-circle";
      const title = document.createElement("span");
      title.className = "title-text";
      title.innerHTML = question.title;
      lbl.appendChild(icon);
      lbl.appendChild(title);
      header.appendChild(lbl);
      outer.appendChild(header);
      if (question.body) {
        const body = document.createElement("div");
        body.className = "panel-body";
        body.appendChild(document.createTextNode(question.body || ""));
        outer.appendChild(body);
      }
      el.appendChild(outer);
    },
    willUnmount: function(question, el) {}
  };

  Survey.CustomWidgetCollection.Instance.addCustomWidget(widget, "type");
}


function initYesNo(Survey) {
  const widget = {
    name: "yesno",
    title: "Yes/No",
    iconName: "icon-radiogroup",
    widgetIsLoaded: function() {
      return true;
    },
    isFit: function(question) {
      return question.getType() === "yesno";
    },
    activatedByChanged: function(activatedBy) {
      Survey.JsonObject.metaData.addClass("yesno", [], null, "empty");
    },
    htmlTemplate: "<div></div>",
    makeButton: function(name, label, value, question) {
      const chk = document.createElement("input");
      chk.type = "radio";
      chk.name = name;
      chk.value = value;
      chk.checked = (question.value === value);
      chk.onclick = function() {
        if ((this as HTMLInputElement).checked)
          question.value = value;
      };
      const outer = document.createElement("label");
      outer.className = "survey-yesno";
      outer.appendChild(chk);
      const div = document.createElement("span");
      div.className = "survey-yesno-button";
      div.appendChild(document.createTextNode(label));
      outer.appendChild(div);
      return outer;
    },
    afterRender: function(question, el) {
      while (el.childNodes.length)
        el.removeChild(el.childNodes[0]);

      const choices: any[] = [
        { label: "Yes", value: "y" },
        { label: "No", value: "n" }
      ];
      for (let opt of choices) {
        const btn = this.makeButton(question.name, opt.label, opt.value, question);
        opt.button = btn;
        el.appendChild(btn);
      }
      question.valueChangedCallback = function() {
        for (let opt of choices) {
          if (opt.value === question.value) opt.button.checked = true;
        }
      };
    },
    willUnmount: function(question, el) {}
  };

  Survey.CustomWidgetCollection.Instance.addCustomWidget(widget, "type");
}


function initAddressBlock(Survey) {
  const widget = {
    name: "address",
    title: "Postal Address",
    iconName: "icon-multipletext",
    widgetIsLoaded: function() {
      return true;
    },
    isFit: function(question) {
      return question.getType() === "address";
    },
    activatedByChanged: function(activatedBy) {
      Survey.JsonObject.metaData.addClass("address", [], null, "empty");
    },
    htmlTemplate: "<div></div>",
    provinceOptions: function() {
      return [
        {
          "value": "AB",
          "text": "Alberta"
        },
        {
          "value": "BC",
          "text": "British Columbia"
        },
        {
          "value": "MB",
          "text": "Manitoba"
        },
        {
          "value": "NB",
          "text": "New Brunswick"
        },
        {
          "value": "NF",
          "text": "Newfoundland and Labrador"
        },
        {
          "value": "NT",
          "text": "Northwest Territories"
        },
        {
          "value": "NS",
          "text": "Nova Scotia"
        },
        {
          "value": "NU",
          "text": "Nunavut"
        },
        {
          "value": "ON",
          "text": "Ontario"
        },
        {
          "value": "PE",
          "text": "Prince Edward Island"
        },
        {
          "value": "QC",
          "text": "Quebec"
        },
        {
          "value": "SK",
          "text": "Saskatchewan"
        },
        {
          "value": "YT",
          "text": "Yukon"
        }
      ];
    },
    countryOptions: function() {
      return [
        {
          "value": "CAN",
          "text": "Canada"
        },
        {
          "value": "USA",
          "text": "USA"
        }
      ];
    },
    afterRender: function(question, el) {
      while (el.childNodes.length)
        el.removeChild(el.childNodes[0]);

      let outer = document.createElement("div");
      let outerCls = "survey-address";
      outer.className = outerCls;

      let row = document.createElement("div");
      row.className = "row survey-address-line";
      let cell = document.createElement("div");
      cell.className = "col-sm-12";
      row.appendChild(cell);
      let addr1 = document.createElement("input");
      addr1.className = "form-control";
      addr1.placeholder = "Street address, for example: 800 Hornby St.";
      cell.appendChild(addr1);
      outer.appendChild(row);

      row = document.createElement("div");
      row.className = "row survey-address-line";
      cell = document.createElement("div");
      cell.className = "col-sm-12";
      row.appendChild(cell);
      let addr2 = document.createElement("input");
      addr2.className = "form-control";
      addr2.placeholder = "Second address line, if needed";
      cell.appendChild(addr2);
      outer.appendChild(row);

      row = document.createElement("div");
      row.className = "row survey-address-line";

      cell = document.createElement("div");
      cell.className = "col-sm-6";
      let label = document.createElement("label");
      // FIXME - set label.for to city ID
      label.className = "survey-address-label";
      label.appendChild(document.createTextNode("City / Town"));
      cell.appendChild(label);
      let city = document.createElement("input");
      city.className = "form-control";
      cell.appendChild(city);
      row.appendChild(cell);

      cell = document.createElement("div");
      cell.className = "col-sm-6";
      label = document.createElement("label");
      // FIXME - set label.for to province ID
      label.className = "survey-address-label";
      label.appendChild(document.createTextNode("Province / State / Region"));
      cell.appendChild(label);
      let state = document.createElement("select");
      state.className = "form-control";
      let stateOpts = this.provinceOptions();
      for (var province of stateOpts) {
        var opt = document.createElement("option");
        opt.text = province.text;
        opt.value = province.value;
        state.appendChild(opt);
      }
      state.value = "BC";
      cell.appendChild(state);
      row.appendChild(cell);

      outer.appendChild(row);

      row = document.createElement("div");
      row.className = "row survey-address-line";

      cell = document.createElement("div");
      cell.className = "col-sm-6";
      label = document.createElement("label");
      // FIXME - set label.for to province ID
      label.className = "survey-address-label";
      label.appendChild(document.createTextNode("Country"));
      cell.appendChild(label);
      let country = document.createElement("select");
      country.className = "form-control";
      let countryOpts = this.countryOptions();
      for (var cval of countryOpts) {
        var opt = document.createElement("option");
        opt.text = cval.text;
        opt.value = cval.value;
        country.appendChild(opt);
      }
      country.value = "CAN";
      cell.appendChild(country);
      row.appendChild(cell);

      cell = document.createElement("div");
      cell.className = "col-sm-6";
      label = document.createElement("label");
      // FIXME - set label.for to postal code ID
      label.className = "survey-address-label";
      label.appendChild(document.createTextNode("Postal Code"));
      cell.appendChild(label);
      let postCode = document.createElement("input");
      postCode.className = "form-control";
      cell.appendChild(postCode);
      row.appendChild(cell);

      outer.appendChild(row);

      el.appendChild(outer);

      question.valueChangedCallback = function() {
        // populate fields
      };
      // question.valueChangedCallback();
    },
    willUnmount: function(question, el) {}
  };

  Survey.CustomWidgetCollection.Instance.addCustomWidget(widget, "type");
}


export function addQuestionTypes(Survey) {
  // fixCheckboxes(Survey); //this breaks IE 11
  initYesNo(Survey);
  initInfoText(Survey);
  initHelpText(Survey);
  initAddressBlock(Survey);
}

export function addToolboxOptions(editor) {
  editor.toolbox.addItem(
    {
      name: "yesno",
      title: "Yes/No Choice",
      isCopied: true,
      iconName: "icon-radiogroup",
      json: {
        type: "yesno"
      }
    }
  );
  editor.toolbox.addItem(
    {
      name: "infotext",
      title: "Info Text",
      isCopied: true,
      iconName: "icon-panel",
      json: {
        type: "infotext",
        titleLocation: "hidden"
      }
    }
  );
  editor.toolbox.addItem(
    {
      name: "helptext",
      title: "Help Text",
      isCopied: true,
      iconName: "icon-panel",
      json: {
        type: "helptext",
        titleLocation: "hidden"
      }
    }
  );
  editor.toolbox.addItem(
    {
      name: "address",
      title: "Postal Address",
      isCopied: true,
      iconName: "icon-multipletext",
      json: {
        type: "address"
      }
    }
  );
}
