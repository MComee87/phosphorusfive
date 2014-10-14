/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */


/*
 * main namespace for unit tests
 */
var tests = {};


/*
 * called when a unit test fissles
 */
tests.setError = function(id) {
  var el = pf.$(id);
  el.el.value = 'failed';
  el.el.className = 'failed';
  el.el.disabled = 'disabled';
};


/*
 * called when a unit test succeeds
 */
tests.setSuccess = function(id) {
  var el = pf.$(id);
  el.el.value = 'success';
  el.el.className = 'success';
  el.el.disabled = 'disabled';
};


/*
 * used to count objects in object
 */
tests.countMembers = function(obj) {
  var count = 0;
  for (var idx in obj) {
    count += 1;
  }
  return count;
};


/*
 * runs all unit tests
 */
tests.run_all = function(id) {
  var els = document.getElementsByTagName('input');
  for (var idx = 0; idx < els.length; idx++) {
    if (els[idx].id.indexOf('invoke') == 0 && els[idx].className == 'undetermined') {
      // this is a unit test
      try {
      tests[els[idx].id]();
      } catch (err) {
        // do nothing, handled in 'onerror' handlers
      }
    }
  }
};


/*
 * invokes an empty event handler, asserting it returns nothing
 */
tests.invoke_empty = function(event) {
  var el = pf.$('sandbox_invoke_empty');
  el.raise('sandbox_invoke_empty_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_empty');
    },

    onsuccess: function(serverReturn, evt) {
      if (!tests.countMembers(serverReturn) == 0) {
        tests.setError('invoke_empty');
      } else {
        tests.setSuccess('invoke_empty');
      }
    }
  });
};


/*
 * invokes an empty event handler that throws an exception, asserting it calls 'onerror'
 */
tests.invoke_exception = function(event) {
  var el = pf.$('sandbox_invoke_exception');
  el.raise('sandbox_invoke_exception_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setSuccess('invoke_exception');
      return true;
    },

    onsuccess: function(serverReturn, evt) {
      tests.setError('invoke_exception');
    }
  });
};


/*
 * invokes a non-existing event handler, asserting 'onerror' is called
 */
tests.invoke_non_existing = function(event) {
  var el = pf.$('sandbox_invoke_non_existing');
  el.raise('sandbox_invoke_non_existing_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setSuccess('invoke_non_existing');
      return true;
    },

    onsuccess: function(serverReturn, evt) {
      tests.setError('invoke_non_existing');
    }
  });
};


/*
 * invokes an event handler not marked as WebMethod, asserting 'onerror' is called
 */
tests.invoke_no_webmethod = function(event) {
  var el = pf.$('sandbox_invoke_no_webmethod');
  el.raise('sandbox_invoke_no_webmethod_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setSuccess('invoke_no_webmethod');
      return true;
    },

    onsuccess: function(serverReturn, evt) {
      tests.setError('invoke_no_webmethod');
    }
  });
};


/*
 * invokes an event handler not marked as WebMethod, asserting 'onerror' is called
 */
tests.invoke_normal = function(event) {
  var el = pf.$('sandbox_invoke_normal');
  el.raise('onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_normal');
    },

    onsuccess: function(serverReturn, evt) {
      tests.setSuccess('invoke_normal');
    }
  });
};


/*
 * invokes an event handler changing content of widget
 */
tests.invoke_change_content = function(event) {
  var el = pf.$('sandbox_invoke_change_content');
  el.raise('sandbox_invoke_change_content_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_change_content');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 2) {
        tests.setError('invoke_change_content');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_change_content.innerHTML != 'new value') {
        tests.setError('invoke_change_content');
        return;
      }
      if (!serverReturn.__pf_change.__VIEWSTATE) {
        tests.setError('invoke_change_content');
        return;
      }
      tests.setSuccess('invoke_change_content');
    }
  });
};


/*
 * invokes an event handler changing two attributes of widget
 */
tests.invoke_change_two_properties = function(event) {
  var el = pf.$('sandbox_invoke_change_two_properties');
  el.raise('sandbox_invoke_change_two_properties_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_change_two_properties');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 2) {
        tests.setError('invoke_change_two_properties');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_change_two_properties.class != 'new value 1') {
        tests.setError('invoke_change_two_properties');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_change_two_properties.innerHTML != 'new value 2') {
        tests.setError('invoke_change_two_properties');
        return;
      }
      if (!serverReturn.__pf_change.__VIEWSTATE) {
        tests.setError('invoke_change_two_properties');
        return;
      }
      tests.setSuccess('invoke_change_two_properties');
    }
  });
};


/*
 * invokes an event handler adding an attribute, then a new event handler removing the same attribute
 */
tests.invoke_add_remove = function(event) {
  var el = pf.$('sandbox_invoke_add_remove');
  el.raise('sandbox_invoke_add_remove_1_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_add_remove');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 2) {
        tests.setError('invoke_add_remove');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_add_remove.class != 'new value 1') {
        tests.setError('invoke_add_remove');
        return;
      }
      if (!serverReturn.__pf_change.__VIEWSTATE) {
        tests.setError('invoke_add_remove');
        return;
      }

      // removing attribute 
      el.raise('sandbox_invoke_add_remove_2_onclick', {
        onerror: function(statusCode, statusText, responseHtml, evt) {
          tests.setError('invoke_add_remove');
        },

        onsuccess: function(serverReturn, evt) {
          if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 2) {
            tests.setError('invoke_add_remove');
            return;
          }
          if (serverReturn.__pf_change.sandbox_invoke_add_remove.__pf_del[0] != 'class') {
            tests.setError('invoke_add_remove');
            return;
          }
          if (!serverReturn.__pf_change.__VIEWSTATE) {
            tests.setError('invoke_add_remove');
            return;
          }
          tests.setSuccess('invoke_add_remove');
        }
      });
    }
  });
};


/*
 * invokes an event handler that adds and removes the same attribute in the same request
 */
tests.invoke_add_remove_same = function(event) {
  var el = pf.$('sandbox_invoke_add_remove_same');
  el.raise('sandbox_invoke_add_remove_same_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_add_remove_same');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 0) {
        tests.setError('invoke_add_remove_same');
        return;
      }

      tests.setSuccess('invoke_add_remove_same');
    }
  });
};


/*
 * changes an attribute twice on the server
 */
tests.invoke_change_twice = function(event) {
  var el = pf.$('sandbox_invoke_change_twice');
  el.raise('sandbox_invoke_change_twice_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_change_twice');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 2) {
        tests.setError('invoke_change_twice');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_change_twice.class != 'jumbo') {
        tests.setError('invoke_change_twice');
        return;
      }

      tests.setSuccess('invoke_change_twice');
    }
  });
};


/*
 * changes an attribute declared in markup on server
 */
tests.invoke_change_markup_attribute = function(event) {
  var el = pf.$('sandbox_invoke_change_markup_attribute');
  el.raise('sandbox_invoke_change_markup_attribute_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_change_markup_attribute');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 2) {
        tests.setError('invoke_change_markup_attribute');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_change_markup_attribute.class != 'bar') {
        tests.setError('invoke_change_markup_attribute');
        return;
      }

      tests.setSuccess('invoke_change_markup_attribute');
    }
  });
};


/*
 * removes an attribute declared in markup on server
 */
tests.invoke_remove_markup_attribute = function(event) {
  var el = pf.$('sandbox_invoke_remove_markup_attribute');
  el.raise('sandbox_invoke_remove_markup_attribute_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_remove_markup_attribute');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 2) {
        tests.setError('invoke_remove_markup_attribute');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_remove_markup_attribute.__pf_del.length != 1) {
        tests.setError('invoke_remove_markup_attribute');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_remove_markup_attribute.__pf_del[0] != 'class') {
        tests.setError('invoke_remove_markup_attribute');
        return;
      }

      tests.setSuccess('invoke_remove_markup_attribute');
    }
  });
};


/*
 * removes an attribute declared in markup on server, then invokes new event handler that adds the same attribute back up again
 */
tests.invoke_remove_add_markup_attribute = function(event) {
  var el = pf.$('sandbox_invoke_remove_add_markup_attribute');
  el.raise('sandbox_invoke_remove_add_markup_attribute_1_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_remove_add_markup_attribute');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 2) {
        tests.setError('invoke_remove_add_markup_attribute');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_remove_add_markup_attribute.__pf_del.length != 1) {
        tests.setError('invoke_remove_add_markup_attribute');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_remove_add_markup_attribute.__pf_del[0] != 'class') {
        tests.setError('invoke_remove_add_markup_attribute');
        return;
      }

      el.raise('sandbox_invoke_remove_add_markup_attribute_2_onclick', {
        onerror: function(statusCode, statusText, responseHtml, evt) {
          tests.setError('invoke_remove_add_markup_attribute');
        },

        onsuccess: function(serverReturn, evt) {
          if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 2) {
            tests.setError('invoke_remove_add_markup_attribute');
            return;
          }
          if (serverReturn.__pf_change.sandbox_invoke_remove_add_markup_attribute.class != 'bar') {
            tests.setError('invoke_remove_add_markup_attribute');
            return;
          }

          tests.setSuccess('invoke_remove_add_markup_attribute');
        }
      });
    }
  });
};


/*
 * concatenate long attribute and verify only changes are returned
 */
tests.invoke_concatenate_long_attribute = function(event) {
  var el = pf.$('sandbox_invoke_concatenate_long_attribute');
  el.raise('sandbox_invoke_concatenate_long_attribute_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_concatenate_long_attribute');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 2) {
        tests.setError('invoke_concatenate_long_attribute');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_concatenate_long_attribute.class.length != 2) {
        tests.setError('invoke_concatenate_long_attribute');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_concatenate_long_attribute.class[0] != 37) {
        tests.setError('invoke_concatenate_long_attribute');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_concatenate_long_attribute.class[1] != 'qwerty') {
        tests.setError('invoke_concatenate_long_attribute');
        return;
      }

      tests.setSuccess('invoke_concatenate_long_attribute');
    }
  });
};


/*
 * create attribute, then concatenate value
 */
tests.invoke_create_concatenate_long_attribute = function(event) {
  var el = pf.$('sandbox_invoke_create_concatenate_long_attribute');
  el.raise('sandbox_invoke_create_concatenate_long_attribute_1_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_create_concatenate_long_attribute');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 2) {
        tests.setError('invoke_create_concatenate_long_attribute');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_create_concatenate_long_attribute.class != 'x1234567890') {
        tests.setError('invoke_create_concatenate_long_attribute');
        return;
      }

      el.raise('sandbox_invoke_create_concatenate_long_attribute_2_onclick', {
        onerror: function(statusCode, statusText, responseHtml, evt) {
          tests.setError('invoke_create_concatenate_long_attribute');
        },

        onsuccess: function(serverReturn, evt) {
          if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 2) {
            tests.setError('invoke_create_concatenate_long_attribute');
            return;
          }
          if (serverReturn.__pf_change.sandbox_invoke_create_concatenate_long_attribute.class.length != 2) {
            tests.setError('invoke_create_concatenate_long_attribute');
            return;
          }
          if (serverReturn.__pf_change.sandbox_invoke_create_concatenate_long_attribute.class[0] != 11) {
            tests.setError('invoke_create_concatenate_long_attribute');
            return;
          }
          if (serverReturn.__pf_change.sandbox_invoke_create_concatenate_long_attribute.class[1] != 'abcdefghijklmnopqrstuvwxyz') {
            tests.setError('invoke_create_concatenate_long_attribute');
            return;
          }

          tests.setSuccess('invoke_create_concatenate_long_attribute');
        }
      });
    }
  });
};


/*
 * change attribute of container's child
 */
tests.invoke_change_container_child = function(event) {
  var el = pf.$('sandbox_invoke_change_container_child_child');
  el.raise('sandbox_invoke_change_container_child_child_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_change_container_child');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 2) {
        tests.setError('invoke_change_container_child');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_change_container_child_child.class == 'bar') {
        tests.setError('invoke_change_container_child');
        return;
      }

      tests.setSuccess('invoke_change_container_child');
    }
  });
};


/*
 * make container widget visible and verify child is also visible
 */
tests.invoke_make_container_visible = function(event) {
  var el = pf.$('sandbox_invoke_make_container_visible');
  el.raise('sandbox_invoke_make_container_visible_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_make_container_visible');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 2) {
        tests.setError('invoke_make_container_visible');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_make_container_visible.outerHTML.indexOf('foo') == -1) {
        tests.setError('invoke_make_container_visible');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_make_container_visible.outerHTML.indexOf('strong') == -1) {
        tests.setError('invoke_make_container_visible');
        return;
      }

      tests.setSuccess('invoke_make_container_visible');
    }
  });
};


/*
 * make container widget visible and verify child is still invisible
 */
tests.invoke_make_container_visible_invisible_child = function(event) {
  var el = pf.$('sandbox_invoke_make_container_visible_invisible_child');
  el.raise('sandbox_invoke_make_container_visible_invisible_child_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_make_container_visible_invisible_child');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 2) {
        tests.setError('invoke_make_container_visible_invisible_child');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_make_container_visible_invisible_child.outerHTML.indexOf('foo') != -1) {
        tests.setError('invoke_make_container_visible_invisible_child');
        return;
      }
      if (serverReturn.__pf_change.sandbox_invoke_make_container_visible_invisible_child.outerHTML.indexOf('strong') != -1) {
        tests.setError('invoke_make_container_visible_invisible_child');
        return;
      }

      tests.setSuccess('invoke_make_container_visible_invisible_child');
    }
  });
};


/*
 * make child invisible, then make container visible
 */
tests.invoke_make_container_visible_child_invisible = function(event) {
  var el = pf.$('sandbox_invoke_make_container_visible_child_invisible');
  el.raise('sandbox_invoke_make_container_visible_child_invisible_1_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_make_container_visible_child_invisible');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 1) {
        tests.setError('invoke_make_container_visible_child_invisible');
        return;
      }

      el.raise('sandbox_invoke_make_container_visible_child_invisible_2_onclick', {
        onerror: function(statusCode, statusText, responseHtml, evt) {
          tests.setError('invoke_make_container_visible_child_invisible');
        },

        onsuccess: function(serverReturn, evt) {
          if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 2) {
            tests.setError('invoke_make_container_visible_child_invisible');
            return;
          }
          if (serverReturn.__pf_change.sandbox_invoke_make_container_visible_child_invisible.outerHTML.indexOf('foo') != -1) {
            tests.setError('invoke_make_container_visible_child_invisible');
            return;
          }
          if (serverReturn.__pf_change.sandbox_invoke_make_container_visible_child_invisible.outerHTML.indexOf('strong') != -1) {
            tests.setError('invoke_make_container_visible_child_invisible');
            return;
          }

          tests.setSuccess('invoke_make_container_visible_child_invisible');
        }
      });
    }
  });
};


/*
 * make child visible, then make container visible
 */
tests.invoke_make_container_visible_child_visible = function(event) {
  var el = pf.$('sandbox_invoke_make_container_visible_child_visible');
  el.raise('sandbox_invoke_make_container_visible_child_visible_1_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_make_container_visible_child_visible');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 1) {
        tests.setError('invoke_make_container_visible_child_visible');
        return;
      }

      el.raise('sandbox_invoke_make_container_visible_child_visible_2_onclick', {
        onerror: function(statusCode, statusText, responseHtml, evt) {
          tests.setError('invoke_make_container_visible_child_visible');
        },

        onsuccess: function(serverReturn, evt) {
          if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__pf_change) != 2) {
            tests.setError('invoke_make_container_visible_child_visible');
            return;
          }
          if (serverReturn.__pf_change.sandbox_invoke_make_container_visible_child_visible.outerHTML.indexOf('foo') == -1) {
            tests.setError('invoke_make_container_visible_child_visible');
            return;
          }
          if (serverReturn.__pf_change.sandbox_invoke_make_container_visible_child_visible.outerHTML.indexOf('strong') == -1) {
            tests.setError('invoke_make_container_visible_child_visible');
            return;
          }

          tests.setSuccess('invoke_make_container_visible_child_visible');
        }
      });
    }
  });
};








