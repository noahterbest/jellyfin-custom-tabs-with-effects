if (typeof window.customTabsPlugin === 'undefined') {
    const STYLE_ID = 'customTabsStyles';

    class StyleManager {
        #element = null;

        get element() {
            if (!this.#element) {
                this.#element = document.getElementById(STYLE_ID) || this.#create();
            }
            return this.#element;
        }

        #create() {
            const style = document.createElement('style');
            style.id = STYLE_ID;
            document.head.appendChild(style);
            return style;
        }

        add(keyframes) {
            this.element.textContent += keyframes;
        }
    }

    class GlowEffect {
        constructor(config, index) {
            this.config = config;
            this.index = index;
            this.color = config.GlowColor || '#00ffff';
            this.intensity = config.GlowIntensity != null ? config.GlowIntensity : 10;
        }

        get blurs() {
            return [this.intensity, this.intensity * 2, this.intensity * 3];
        }

        static boxShadow(blurs, color) {
            return blurs.map(b => `0 0 ${b}px ${color}`).join(', ');
        }

        applyStatic(button) {
            button.style.boxShadow = this.constructor.boxShadow(this.blurs, this.color);
        }
    }

    class HeartbeatEffect extends GlowEffect {
        constructor(config, index) {
            super(config, index);
            this.speed = config.HeartbeatSpeed != null ? config.HeartbeatSpeed : 800;
        }

        get pulseBlurs() {
            return this.blurs.map(b => Math.round(b * 1.5));
        }

        apply(button, styleManager) {
            const animName = `customTabHeartbeat_${this.index}`;
            const baseShadow = GlowEffect.boxShadow(this.blurs, this.color);
            const pulseShadow = GlowEffect.boxShadow(this.pulseBlurs, this.color);
            styleManager.add(
                `@keyframes ${animName}{0%,100%{box-shadow:${baseShadow}}50%{box-shadow:${pulseShadow}}}`
            );
            button.style.animation = `${animName} ${this.speed}ms infinite ease-in-out`;
        }
    }

    class RgbEffect extends GlowEffect {
        constructor(config, index) {
            super(config, index);
            this.speed = config.RgbSpeed != null ? config.RgbSpeed : 2000;
        }

        apply(button, styleManager) {
            const animName = `customTabRgb_${this.index}`;
            const stops = 12;
            let keyframeStops = '';

            for (let s = 0; s <= stops; s++) {
                const hue = (360 / stops) * s;
                const pct = (s / stops) * 100;
                const shadow = GlowEffect.boxShadow(this.blurs, `hsl(${hue},100%,50%)`);
                keyframeStops += `${pct}%{box-shadow:${shadow}}`;
            }

            styleManager.add(`@keyframes ${animName}{${keyframeStops}}`);
            button.style.animation = `${animName} ${this.speed}ms infinite linear`;
        }
    }

    class TabCreator {
        constructor(styleManager) {
            this.styleManager = styleManager;
        }

        create(config, index) {
            const button = this.#createButton(config, index);
            this.#applyEffects(button, config, index);
            return button;
        }

        #createButton(config, index) {
            const title = document.createElement('div');
            title.classList.add('emby-button-foreground');
            title.innerText = config.Title;

            const button = document.createElement('button');
            button.type = 'button';
            button.setAttribute('is', 'empty-button');
            button.classList.add('emby-tab-button', 'emby-button');
            button.setAttribute('data-index', index + 2);
            button.setAttribute('id', `customTabButton_${index}`);
            button.appendChild(title);

            return button;
        }

        #applyEffects(button, config, index) {
            if (!config.GlowEnabled) return;

            if (config.RgbAnimationEnabled) {
                new RgbEffect(config, index).apply(button, this.styleManager);
            } else if (config.HeartbeatEnabled) {
                new HeartbeatEffect(config, index).apply(button, this.styleManager);
            } else {
                new GlowEffect(config, index).applyStatic(button);
            }
        }
    }

    class CustomTabsPlugin {
        constructor() {
            this.styleManager = new StyleManager();
            this.tabCreator = new TabCreator(this.styleManager);
        }

        init() {
            console.log('CustomTabs: Initializing plugin');
            this.waitForUI();
        }

        waitForUI() {
            if (!this.#isHomePage()) return;

            if (typeof ApiClient !== 'undefined' && document.querySelector('.emby-tabs-slider')) {
                console.debug('CustomTabs: UI elements available on main page, creating tabs');
                this.createCustomTabs();
            } else {
                console.debug('CustomTabs: Waiting for UI elements on main page...');
                setTimeout(() => this.waitForUI(), 200);
            }
        }

        #isHomePage() {
            const hash = window.location.hash;
            if (hash !== '' && hash !== '#/home' && hash !== '#/home.html' &&
                !hash.includes('#/home?') && !hash.includes('#/home.html?')) {
                console.debug('CustomTabs: Not on main page, skipping UI check. Hash:', hash);
                return false;
            }
            return true;
        }

        createCustomTabs() {
            console.debug('CustomTabs: Starting tab creation process');

            const tabsSlider = document.querySelector('.emby-tabs-slider');
            if (!tabsSlider || tabsSlider.querySelector('[id^="customTabButton_"]')) {
                return;
            }

            ApiClient.fetch({
                url: ApiClient.getUrl('CustomTabs/Config'),
                type: 'GET',
                dataType: 'json',
                headers: { accept: 'application/json' }
            }).then((configs) => {
                console.debug('CustomTabs: Retrieved config for', configs.length, 'tabs');

                const tabsSlider = document.querySelector('.emby-tabs-slider');
                if (!tabsSlider) return;

                configs.forEach((config, i) => {
                    if (document.querySelector(`#customTabButton_${i}`)) return;

                    console.log('CustomTabs: Creating custom tab:', config.Title);
                    const button = this.tabCreator.create(config, i);
                    tabsSlider.appendChild(button);
                    console.log(`CustomTabs: Added tab customTabButton_${i} to tabs slider`);
                });

                console.log('CustomTabs: All custom tabs created successfully');
            }).catch((error) => {
                console.error('CustomTabs: Error fetching tab configs:', error);
            });
        }
    }

    window.customTabsPlugin = new CustomTabsPlugin();

    const handleNavigation = () => {
        console.debug('CustomTabs: Navigation detected, re-initializing after delay');
        setTimeout(() => window.customTabsPlugin.init(), 800);
    };

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => window.customTabsPlugin.init());
    } else {
        window.customTabsPlugin.init();
    }

    window.addEventListener('popstate', handleNavigation);
    window.addEventListener('pageshow', handleNavigation);
    window.addEventListener('focus', handleNavigation);

    const originalPushState = history.pushState;
    history.pushState = function () {
        originalPushState.apply(history, arguments);
        handleNavigation();
    };

    const originalReplaceState = history.replaceState;
    history.replaceState = function () {
        originalReplaceState.apply(history, arguments);
        handleNavigation();
    };

    document.addEventListener('visibilitychange', () => {
        if (!document.hidden) {
            console.debug('CustomTabs: Page became visible, checking for tabs');
            setTimeout(() => window.customTabsPlugin.init(), 300);
        }
    });

    let touchNavigation = false;
    document.addEventListener('touchstart', () => { touchNavigation = true; });
    document.addEventListener('touchend', () => {
        if (touchNavigation) {
            setTimeout(() => window.customTabsPlugin.init(), 1000);
            touchNavigation = false;
        }
    });

    console.log('CustomTabs: Plugin setup complete');
}
