/**
 * Post-processes Tailwind's output to scope global CSS custom property
 * declarations into .cc-isolation-container, preventing them from
 * leaking into consuming projects.
 *
 * Tailwind generates two separate rules in unminified CSS:
 *   1. "*, ::before, ::after { --tw-* ... }"
 *   2. "::backdrop { --tw-* ... }"
 * The minifier combines them into one: "*,::backdrop,:after,:before{...}"
 */
const fs = require('fs');
const path = require('path');

const inputFile = process.argv[2];
const outputFile = process.argv[3] || inputFile;

if (!inputFile) {
    console.error('Usage: node postprocess-css.js <input.css> [output.css]');
    process.exit(1);
}

let css = fs.readFileSync(inputFile, 'utf8');
let changed = false;

// Handle unminified CSS: two separate rules
// Rule 1: "*, ::before, ::after {" → scope it
const rule1Pattern = /^(\*)\s*,\s*(::before|:before)\s*,\s*(::after|:after)\s*\{/gm;
if (rule1Pattern.test(css)) {
    rule1Pattern.lastIndex = 0;
    css = css.replace(rule1Pattern,
        '.cc-isolation-container,.cc-isolation-container *,.cc-isolation-container ::before,.cc-isolation-container ::after{');
    changed = true;
}

// Rule 2: "::backdrop {" → scope it
const rule2Pattern = /^(::backdrop)\s*\{/gm;
if (rule2Pattern.test(css)) {
    rule2Pattern.lastIndex = 0;
    css = css.replace(rule2Pattern,
        '.cc-isolation-container ::backdrop{');
    changed = true;
}

// Handle minified CSS: combined rule "*,::backdrop,:after,:before{"
if (!changed) {
    const minifiedPattern = /(\*)\s*,\s*(::backdrop)\s*,\s*(:after|::after)\s*,\s*(:before|::before)\s*\{/g;
    if (minifiedPattern.test(css)) {
        minifiedPattern.lastIndex = 0;
        css = css.replace(minifiedPattern,
            '.cc-isolation-container,.cc-isolation-container *,.cc-isolation-container ::backdrop,.cc-isolation-container :after,.cc-isolation-container :before{');
        changed = true;
    }
}

fs.writeFileSync(outputFile, css, 'utf8');

if (changed) {
    console.log('Scoped global --tw-* rules into .cc-isolation-container');
} else {
    console.log('No global --tw-* rules found (already scoped or not present)');
}
console.log(`Done: ${path.basename(outputFile)}`);
