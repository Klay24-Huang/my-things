/*
* SelectMultiple v0.1
* Copyright (c) 2015 krazedkrish( Shalil Awaley )
*
* This program is free software. It comes without any warranty, to
* the extent permitted by applicable law. You can redistribute it
* and/or modify it under the termx of the MIT LICENSE.
* See https://en.wikipedia.org/wiki/MIT_License for more details.
*/

!function ($) {

  "use strict";


 /* SELECTMULTIPLE CLASS DEFINITION
  * ====================== */

  var SelectMultiple = function (element, options) {
    this.options = options;
    this.$element = $(element);
    this.$container = $('<div/>', { 'class': "mx-container" });
    this.$selectableContainer = $('<div/>', { 'class': 'mx-selectable' });
    this.$selectableUl = $('<ul/>', { 'class': "mx-list", 'tabindex' : '-1' });
    this.scrollTo = 0;
    this.elemxSelector = 'li';
  };

  SelectMultiple.prototype = {
    constructor: SelectMultiple,

    init: function(){
      var that = this,
          mx = this.$element;

      if (mx.next('.mx-container').length === 0){
        mx.css({ position: 'absolute', left: '-9999px' });
        mx.attr('id', mx.attr('id') ? mx.attr('id') : Math.ceil(Math.random()*1000)+'selectmultiple');
        this.$container.attr('id', 'mx-'+mx.attr('id'));
        this.$container.addClass(that.options.cssClass);
        mx.find('option').each(function(){
          that.generateLisFromOption(this);
        });


        if (that.options.selectableHeader){
          that.$selectableContainer.append(that.options.selectableHeader);
        }
        that.$selectableContainer.append(that.$selectableUl);
        if (that.options.selectableFooter){
          that.$selectableContainer.append(that.options.selectableFooter);
        }


        that.$container.append(that.$selectableContainer);
        mx.after(that.$container);

        that.activeMouse(that.$selectableUl);
        that.activeKeyboard(that.$selectableUl);

        var action = that.options.dblClick ? 'dblclick' : 'click';

        that.$selectableUl.on(action, '.mx-elem-selectable', function(){
          that.select($(this).data('mx-value'));
        });


        mx.on('focus', function(){
          that.$selectableUl.focus();
        })
      }

      var selectedValues = mx.find('option:selected').map(function(){ return $(this).val(); }).get();
      that.select(selectedValues, 'init');

      if (typeof that.options.afterInit === 'function') {
        that.options.afterInit.call(this, this.$container);
      }
    },

    'generateLisFromOption' : function(option, index, $container){
      var that = this,
          mx = that.$element,
          attributes = "",
          $option = $(option);

      for (var cpt = 0; cpt < option.attributes.length; cpt++){
        var attr = option.attributes[cpt];

        if(attr.name !== 'value' && attr.name !== 'disabled'){
          attributes += attr.name+'="'+attr.value+'" ';
        }
      }
      var selectableLi = $('<li '+attributes+'><span>'+(that.options.allowHTML === true ? $option.text() : that.escapeHTML($option.text()))+'</span><span class="pull-right mx-elem-selected">✔</span></li>'),
          selectedLi = selectableLi.clone(),
          value = $option.val(),
          elementId = that.sanitize(value);

      selectableLi.children('.mx-elem-selected').hide();

      selectableLi
        .data('mx-value', value)
        .addClass('mx-elem-selectable')
        .attr('id', elementId+'-selectable');


      if ($option.prop('disabled') || mx.prop('disabled')){
        selectableLi.addClass(that.options.disabledClass);
      }

      var $optgroup = $option.parent('optgroup');

      if ($optgroup.length > 0){
        var optgroupLabel = $optgroup.attr('label'),
            optgroupId = that.sanitize(optgroupLabel),
            $selectableOptgroup = that.$selectableUl.find('#optgroup-selectable-'+optgroupId);

        if ($selectableOptgroup.length === 0){
          var optgroupContainerTpl = '<li class="mx-optgroup-container"></li>',
              optgroupTpl = '<ul class="mx-optgroup"><li class="mx-optgroup-label"><span>'+optgroupLabel+'</span></li></ul>';

          $selectableOptgroup = $(optgroupContainerTpl);
          $selectableOptgroup.attr('id', 'optgroup-selectable-'+optgroupId);
          $selectableOptgroup.append($(optgroupTpl));
          if (that.options.selectableOptgroup){
            $selectableOptgroup.find('.mx-optgroup-label').on('click', function(){
              var values = $optgroup.children(':not(:selected, :disabled)').map(function(){ return $(this).val() }).get();
              that.select(values);
            });
          }
          that.$selectableUl.append($selectableOptgroup);
        }
        index = index == undefined ? $selectableOptgroup.find('ul').children().length : index + 1;
        selectableLi.insertAt(index, $selectableOptgroup.children());
      } else {
        index = index == undefined ? that.$selectableUl.children().length : index;

        selectableLi.insertAt(index, that.$selectableUl);
      }
    },

    'addOption' : function(options){
      var that = this;

      if (options.value !== undefined && options.value !== null){
        options = [options];
      }
      $.each(options, function(index, option){
        if (option.value !== undefined && option.value !== null &&
            that.$element.find("option[value='"+option.value+"']").length === 0){
          var $option = $('<option value="'+option.value+'">'+option.text+'</option>'),
              index = parseInt((typeof option.index === 'undefined' ? that.$element.children().length : option.index)),
              $container = option.nested == undefined ? that.$element : $("optgroup[label='"+option.nested+"']")

          $option.insertAt(index, $container);
          that.generateLisFromOption($option.get(0), index, option.nested);
        }
      })
    },

    'escapeHTML' : function(text){
      return $("<div>").text(text).html();
    },

    'activeKeyboard' : function($list){
      var that = this;

      $list.on('focus', function(){
        $(this).addClass('mx-focus');
      })
      .on('blur', function(){
        $(this).removeClass('mx-focus');
      })
      .on('keydown', function(e){
        switch (e.which) {
          case 40:
          case 38:
            e.preventDefault();
            e.stopPropagation();
            that.moveHighlight($(this), (e.which === 38) ? -1 : 1);
            return;
          case 9:
            if(that.$element.is('[tabindex]')){
              e.preventDefault();
              var tabindex = parseInt(that.$element.attr('tabindex'), 10);
              tabindex = (e.shiftKey) ? tabindex-1 : tabindex+1;
              $('[tabindex="'+(tabindex)+'"]').focus();
              return;
            }else{
              if(e.shiftKey){
                that.$element.trigger('focus');
              }
            }
        }
        if($.inArray(e.which, that.options.keySelect) > -1){
          e.preventDefault();
          e.stopPropagation();
          that.selectHighlighted($list);
          return;
        }
      });
    },

    'moveHighlight': function($list, direction){
      var $elemx = $list.find(this.elemxSelector),
          $currElem = $elemx.filter('.mx-hover'),
          $nextElem = null,
          elemHeight = $elemx.first().outerHeight(),
          containerHeight = $list.height(),
          containerSelector = '#'+this.$container.prop('id');

      $elemx.removeClass('mx-hover');
      if (direction === 1){ // DOWN

        $nextElem = $currElem.nextAll(this.elemxSelector).first();
        if ($nextElem.length === 0){
          var $optgroupUl = $currElem.parent();

          if ($optgroupUl.hasClass('mx-optgroup')){
            var $optgroupLi = $optgroupUl.parent(),
                $nextOptgroupLi = $optgroupLi.next(':visible');

            if ($nextOptgroupLi.length > 0){
              $nextElem = $nextOptgroupLi.find(this.elemxSelector).first();
            } else {
              $nextElem = $elemx.first();
            }
          } else {
            $nextElem = $elemx.first();
          }
        }
      } else if (direction === -1){ // UP

        $nextElem = $currElem.prevAll(this.elemxSelector).first();
        if ($nextElem.length === 0){
          var $optgroupUl = $currElem.parent();

          if ($optgroupUl.hasClass('mx-optgroup')){
            var $optgroupLi = $optgroupUl.parent(),
                $prevOptgroupLi = $optgroupLi.prev(':visible');

            if ($prevOptgroupLi.length > 0){
              $nextElem = $prevOptgroupLi.find(this.elemxSelector).last();
            } else {
              $nextElem = $elemx.last();
            }
          } else {
            $nextElem = $elemx.last();
          }
        }
      }
      if ($nextElem.length > 0){
        $nextElem.addClass('mx-hover');
        var scrollTo = $list.scrollTop() + $nextElem.position().top -
                       containerHeight / 2 + elemHeight / 2;

        $list.scrollTop(scrollTo);
      }
    },

    'selectHighlighted' : function($list){
      var $elemx = $list.find(this.elemxSelector),
          $highlightedElem = $elemx.filter('.mx-hover').first();

      if ($highlightedElem.length > 0){
        if ($list.parent().hasClass('mx-selectable')){
          this.select($highlightedElem.data('mx-value'));
        } else {
          this.deselect($highlightedElem.data('mx-value'));
        }
        $elemx.removeClass('mx-hover');
      }
    },

    'activeMouse' : function($list){
      var that = this;

      $('body').on('mouseenter', that.elemxSelector, function(){
        $(this).parents('.mx-container').find(that.elemxSelector).removeClass('mx-hover');
        $(this).addClass('mx-hover');
      });

      $('body').on('mouseleave', that.elemxSelector, function () {
          $(this).parents('.mx-container').find(that.elemxSelector).removeClass('mx-hover');;
      });
    },

    'refresh' : function() {
      this.destroy();
      this.$element.selectMultiple(this.options);
    },

    'destroy' : function(){
      $("#mx-"+this.$element.attr("id")).remove();
      this.$element.css('position', '').css('left', '')
      this.$element.removeData('selectmultiple');
    },

    'select' : function(value, method){
      if (typeof value === 'string'){ value = [value]; }

      var that = this,
          mx = this.$element,
          mxIds = $.map(value, function(val){ return(that.sanitize(val)); }),
          selectables = this.$selectableUl.find('#' + mxIds.join('-selectable, #')+'-selectable').filter(':not(.'+that.options.disabledClass+')'),
          options = mx.find('option:not(:disabled)').filter(function(){ return($.inArray(this.value, value) > -1); });

      if (method === 'init'){
        selectables = this.$selectableUl.find('#' + mxIds.join('-selectable, #')+'-selectable');
      }

      if (selectables.length > 0){
        selectables.addClass('mx-selected').children('.mx-elem-selected').show();

        if (method !== 'init' && options.prop('selected') == true) {
          that.deselect(value, method);
          return;
        }

        options.prop('selected', true);

        var selectableOptgroups = that.$selectableUl.children('.mx-optgroup-container');
        if (selectableOptgroups.length > 0){
          selectableOptgroups.each(function(){
            var selectablesLi = $(this).find('.mx-elem-selectable');
            if (that.options.hideOptGroupLabelOnAllSelected !== false && selectablesLi.length === selectablesLi.filter('.mx-selected').length){
              $(this).find('.mx-optgroup-label').hide();
            }
          });

        }
        if (method !== 'init'){
          mx.trigger('change');
          if (typeof that.options.afterSelect === 'function') {
            that.options.afterSelect.call(this, value);
          }
        }
      }
    },

    'deselect' : function(value, method){
      if (typeof value === 'string'){ value = [value]; }

      var that = this,
          mx = this.$element,
          mxIds = $.map(value, function(val){ return(that.sanitize(val)); }),
          selectables = this.$selectableUl.find('#' + mxIds.join('-selectable, #')+'-selectable'),
          options = mx.find('option').filter(function(){ return($.inArray(this.value, value) > -1); });

      selectables.removeClass('mx-selected').children('.mx-elem-selected').hide();
      options.prop('selected', false);

      var selectableOptgroups = that.$selectableUl.children('.mx-optgroup-container');
      if (selectableOptgroups.length > 0){
        selectableOptgroups.each(function(){
          var selectablesLi = $(this).find('.mx-elem-selectable');
          if (selectablesLi.filter(':not(.mx-selected)').length > 0){
            $(this).find('.mx-optgroup-label').show();
          }
        });

      }
      if (method !== 'init'){
        mx.trigger('change');
        if (typeof that.options.afterDeselect === 'function') {
          that.options.afterDeselect.call(this, value);
        }
      }
    },

    'select_all' : function(){
      var mx = this.$element,
          values = mx.val();

      mx.find('option:not(":disabled")').prop('selected', true);
      this.$selectableUl.find('.mx-elem-selectable').filter(':not(.'+this.options.disabledClass+')').addClass('mx-selected').children('.mx-elem-selected').show();
      if(this.options.hideOptGroupLabelOnAllSelected !== false){
         this.$selectableUl.find('.mx-optgroup-label').hide(); 
      }
      mx.trigger('change');
      if (typeof this.options.afterSelect === 'function') {
        var selectedValues = $.grep(mx.val(), function(item){
          return $.inArray(item, values) < 0;
        });
        this.options.afterSelect.call(this, selectedValues);
      }
    },

    'deselect_all' : function(){
      var mx = this.$element,
          values = mx.val();

      mx.find('option').prop('selected', false);
      this.$selectableUl.find('.mx-elem-selectable').removeClass('mx-selected').children('.mx-elem-selected').hide();
      this.$selectableUl.find('.mx-optgroup-label').show();
      this.$selectableUl.focus();
      mx.trigger('change');
      if (typeof this.options.afterDeselect === 'function') {
        this.options.afterDeselect.call(this, values);
      }
    },

    sanitize: function(value){
      var hash = 0, i, character;
      if (value.length == 0) return hash;
      var ls = 0;
      for (i = 0, ls = value.length; i < ls; i++) {
        character  = value.charCodeAt(i);
        hash  = ((hash<<5)-hash)+character;
        hash |= 0; // Convert to 32bit integer
      }
      return hash;
    }
  };

  /* SELECTMULTIPLE PLUGIN DEFINITION
   * ======================= */

  $.fn.selectMultiple = function () {
    var option = arguments[0],
        args = arguments;

    return this.each(function () {
      var $this = $(this),
          data = $this.data('selectmultiple'),
          options = $.extend({}, $.fn.selectMultiple.defaults, $this.data(), typeof option === 'object' && option);

      if (!data){ $this.data('selectmultiple', (data = new SelectMultiple(this, options))); }

      if (typeof option === 'string'){
        data[option](args[1]);
      } else {
        data.init();
      }
    });
  };

  $.fn.selectMultiple.defaults = {
    keySelect: [32],
    selectableOptgroup: false,
    disabledClass : 'disabled',
    dblClick : false,
    cssClass: '',
    hideOptGroupLabelOnAllSelected: true
  };

  $.fn.selectMultiple.Constructor = SelectMultiple;

  $.fn.insertAt = function(index, $parent) {
    return this.each(function() {
      if (index === 0) {
        $parent.prepend(this);
      } else {
        $parent.children().eq(index - 1).after(this);
      }
    });
}

}(window.jQuery);
