/* ---------------------------------------------------------------------------
   Lead capture for the static site.

   The site has no backend, so every form posts to a hosted form service. One
   config block below is the only thing that needs editing — see LEADS.md.

   Rule this file exists to enforce: never tell a visitor their message was
   sent unless the request actually succeeded. The previous contact form
   printed a success message after a setTimeout and dropped the enquiry, so
   every failure path here reports honestly and offers an email fallback.
--------------------------------------------------------------------------- */
(function () {
  'use strict';

  /* ======================= CONFIGURE THIS ================================ */
  var CONFIG = {
    // Web3Forms access key. Get one free at https://web3forms.com — it is
    // bound to your email address, so no address needs to appear here.
    accessKey: '',

    endpoint: 'https://api.web3forms.com/submit',

    // Shown when a send fails, so a lead can still reach you.
    fallbackEmail: 'zain.tahir512@gmail.com'
  };
  /* ======================================================================= */

  var configured = CONFIG.accessKey && CONFIG.accessKey.length > 10;

  function t(key, fallback) {
    if (window.ZatI18n && typeof window.ZatI18n.get === 'function') {
      var v = window.ZatI18n.get(key);
      if (v) return v;
    }
    return fallback;
  }

  function validEmail(value) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/.test((value || '').trim());
  }

  function setStatus(el, message, state) {
    if (!el) return;
    el.textContent = message;
    el.className = 'lead-status' + (state ? ' is-' + state : '');
  }

  /** Mailto link used whenever the service is unreachable or unconfigured. */
  function mailtoLink(subject, body) {
    return 'mailto:' + CONFIG.fallbackEmail +
      '?subject=' + encodeURIComponent(subject) +
      '&body=' + encodeURIComponent(body);
  }

  function offerFallback(statusEl, subject, body) {
    if (!statusEl) return;
    statusEl.innerHTML = '';
    var span = document.createElement('span');
    span.textContent = t('ld.failed', 'That did not go through. ');
    var a = document.createElement('a');
    a.href = mailtoLink(subject, body);
    a.textContent = t('ld.emailInstead', 'Email it to me directly instead.');
    statusEl.appendChild(span);
    statusEl.appendChild(a);
    statusEl.className = 'lead-status is-error';
  }

  /**
   * Posts a payload to the form service.
   * Resolves true only on a confirmed success response.
   */
  function send(payload) {
    return fetch(CONFIG.endpoint, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' },
      body: JSON.stringify(Object.assign({ access_key: CONFIG.accessKey }, payload))
    })
      .then(function (res) { return res.json().catch(function () { return {}; }); })
      .then(function (data) { return data && data.success === true; })
      .catch(function () { return false; });
  }

  /**
   * Wires one form. opts.collect returns the payload, opts.subject names the
   * email, opts.success is the message shown only on a real success.
   */
  function wire(form, opts) {
    if (!form) return;
    var statusEl = form.querySelector('.lead-status');
    var button = form.querySelector('button[type=submit]');
    var original = button ? button.innerHTML : '';

    form.addEventListener('submit', function (e) {
      e.preventDefault();

      // Honeypot: a hidden field only a bot fills. Pretend success and drop it.
      var hp = form.querySelector('input[name=botcheck]');
      if (hp && hp.value) { form.reset(); return; }

      var payload = opts.collect(form);
      if (!validEmail(payload.email)) {
        setStatus(statusEl, t('ld.badEmail', 'Please enter a valid email address.'), 'error');
        return;
      }

      var bodyText = Object.keys(payload).map(function (k) {
        return k + ': ' + payload[k];
      }).join('\n');

      if (!configured) {
        // No key yet. Say so plainly rather than faking a send.
        offerFallback(statusEl, opts.subject, bodyText);
        return;
      }

      if (button) {
        button.disabled = true;
        button.innerHTML = '<i class="fas fa-spinner fa-spin"></i> ' + t('ld.sending', 'Sending…');
      }
      setStatus(statusEl, '', '');

      send(Object.assign({ subject: opts.subject, from_name: 'zainabbastahir.com' }, payload))
        .then(function (ok) {
          if (ok) {
            setStatus(statusEl, t(opts.successKey, opts.success), 'ok');
            form.reset();
            if (typeof opts.onSuccess === 'function') opts.onSuccess(form);
          } else {
            offerFallback(statusEl, opts.subject, bodyText);
          }
        })
        .finally(function () {
          if (button) { button.disabled = false; button.innerHTML = original; }
        });
    });
  }

  function init() {
    /* ---- contact form ---- */
    wire(document.getElementById('contact-form'), {
      subject: 'Portfolio enquiry',
      successKey: 'ld.contactOk',
      success: 'Thank you — your message reached me. I reply within 24 hours.',
      collect: function (f) {
        return {
          name: (f.querySelector('#cf-name') || {}).value || '',
          email: (f.querySelector('#cf-email') || {}).value || '',
          enquiry_subject: (f.querySelector('#cf-subject') || {}).value || '',
          message: (f.querySelector('#cf-msg') || {}).value || '',
          source: 'contact form'
        };
      }
    });

    /* ---- newsletter ---- */
    document.querySelectorAll('form[data-lead=newsletter]').forEach(function (f) {
      wire(f, {
        subject: 'New subscriber',
        successKey: 'ld.subOk',
        success: 'You are on the list — thank you.',
        collect: function (form) {
          return {
            email: (form.querySelector('input[type=email]') || {}).value || '',
            source: form.getAttribute('data-source') || 'newsletter'
          };
        }
      });
    });

    /* ---- calculator results ----
       The visitor's own slider values travel with the email, so each lead
       arrives already carrying the scale of the problem it represents. */
    wire(document.getElementById('calc-lead'), {
      subject: 'AI spend calculator — report requested',
      successKey: 'ld.calcOk',
      success: 'Sent. Your breakdown is on its way.',
      collect: function (f) {
        var val = function (id) {
          var el = document.getElementById(id);
          return el ? el.textContent.trim() : '';
        };
        return {
          email: (f.querySelector('input[type=email]') || {}).value || '',
          queries_per_day: val('c-q-o'),
          tokens_per_query: val('c-t-o'),
          teams: val('c-team-o'),
          ungoverned_monthly: val('c-before'),
          governed_monthly: val('c-after'),
          saving: val('c-save'),
          source: 'spend calculator'
        };
      }
    });

    // Surface the unconfigured state in the console for whoever deploys it.
    if (!configured && window.console) {
      console.warn('[leads] No access key set in lead.js — forms will offer an ' +
                   'email fallback instead of submitting. See LEADS.md.');
    }
  }

  if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', init);
  else init();
})();
