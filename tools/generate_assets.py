#!/usr/bin/env python3
"""
GOBLIN CHAIN — procedural asset generator.

Generates every audio and 2D art asset the game needs from code, so the
project has zero licensing exposure and everything can be re-rolled by
tweaking parameters:

  sounds/*.wav             16 sound effects (the .sound stubs reference these)
  sounds/music_*.wav       4 phase-music chiptune loops (seamless, whole bars)
  materials/posters/*.png  8 poster background art pieces (WorldPanel text
                           renders on top, so these stay low-contrast)

Run from the repo root:  python3 tools/generate_assets.py
Requires: numpy, pillow.
"""

import math
import os
import wave

import numpy as np
from PIL import Image, ImageDraw

SR = 22050
ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SOUNDS = os.path.join(ROOT, "sounds")
POSTERS = os.path.join(ROOT, "materials", "posters")

rng = np.random.default_rng(6969)  # deterministic goblin


# ──────────────────────────────────────────────
#  Audio primitives
# ──────────────────────────────────────────────

def t(dur):
    return np.linspace(0, dur, int(SR * dur), endpoint=False)


def sine(freq, dur):
    return np.sin(2 * np.pi * freq * t(dur))


def square(freq, dur, duty=0.5):
    return np.where((t(dur) * freq) % 1.0 < duty, 1.0, -1.0)


def saw(freq, dur):
    return 2.0 * ((t(dur) * freq) % 1.0) - 1.0


def tri(freq, dur):
    return 2.0 * np.abs(saw(freq, dur)) - 1.0


def noise(dur):
    return rng.uniform(-1, 1, int(SR * dur))


def env(sig, attack=0.005, release=None, curve=3.0):
    """Attack + exponential decay envelope over the whole signal."""
    n = len(sig)
    e = np.exp(-curve * np.linspace(0, 1, n))
    a = int(SR * attack)
    if a > 0:
        e[:a] *= np.linspace(0, 1, a)
    if release:
        r = int(SR * release)
        e[-r:] *= np.linspace(1, 0, r)
    return sig * e


def sweep(f0, f1, dur, wave_fn=np.sin):
    """Frequency sweep from f0 to f1."""
    tt = t(dur)
    phase = 2 * np.pi * (f0 * tt + (f1 - f0) * tt * tt / (2 * dur))
    return wave_fn(phase)


def mix(*parts):
    n = max(len(p) for p in parts)
    out = np.zeros(n)
    for p in parts:
        out[: len(p)] += p
    return out


def concat(*parts):
    return np.concatenate(parts)


def silence(dur):
    return np.zeros(int(SR * dur))


def normalize(sig, peak=0.82):
    m = np.max(np.abs(sig))
    if m > 0:
        sig = sig / m * peak
    return np.tanh(sig * 1.2) / np.tanh(1.2)  # gentle soft-clip glue


def write_wav(name, sig):
    path = os.path.join(SOUNDS, name)
    data = (normalize(sig) * 32767).astype(np.int16)
    with wave.open(path, "wb") as w:
        w.setnchannels(1)
        w.setsampwidth(2)
        w.setframerate(SR)
        w.writeframes(data.tobytes())
    print(f"  {name:28s} {len(sig)/SR:6.2f}s")


# ──────────────────────────────────────────────
#  Sound effects
# ──────────────────────────────────────────────

def sfx_market_crash():
    boom = env(sine(55, 1.8) + 0.5 * sine(41, 1.8), curve=2.5)
    fall = env(sweep(420, 48, 1.6, lambda p: np.sign(np.sin(p))) * 0.5, curve=2.0)
    debris = env(noise(1.2) * 0.6, curve=5.0)
    return mix(boom, fall, debris)


def sfx_market_moon():
    notes = [261.6, 329.6, 392.0, 523.3, 659.3, 784.0]
    steps = [env(square(f, 0.16, 0.3) * 0.5, curve=4) for f in notes]
    shimmer = env(sweep(1200, 3200, 1.0) * 0.15, curve=2)
    return mix(concat(*steps), concat(silence(0.5), shimmer))


def sfx_event_positive():
    return mix(env(sine(880, 0.7), curve=6),
               concat(silence(0.12), env(sine(1318.5, 0.55) * 0.7, curve=6)))


def sfx_event_negative():
    wob = sine(6, 0.8) * 30
    tt = t(0.8)
    sig = np.sign(np.sin(2 * np.pi * (150 + wob) * tt))
    return env(sig * 0.7, curve=4)


def sfx_notification():
    return concat(env(sine(1240, 0.14), curve=8),
                  env(sine(1860, 0.2), curve=8))


def sfx_phase_transition():
    chord = mix(saw(130.8, 1.4), saw(196.0, 1.4), saw(261.6, 1.4)) * 0.3
    swell = chord * np.linspace(0, 1, len(chord)) ** 2
    whoosh = env(sweep(300, 1800, 1.1) * noise(1.1) * 0.25, curve=1.5)
    return mix(swell * np.exp(-2 * np.linspace(0, 1, len(swell)) ** 4), whoosh)


def sfx_pump_group():
    parts, f, gap = [], 220.0, 0.11
    for _ in range(9):
        parts.append(env(square(f, gap, 0.4) * 0.6, curve=7))
        f *= 1.18
        gap *= 0.92
    return concat(*parts)


def sfx_rig_placed():
    thunk = env(sine(75, 0.4) + 0.4 * sine(52, 0.4), curve=8)
    click = env(noise(0.05), curve=10)
    return mix(thunk, concat(silence(0.02), click * 0.4))


def sfx_rig_upgrade():
    notes = [330, 415, 494, 659]
    return concat(*[env(square(f, 0.18, 0.35) * 0.6, curve=6) for f in notes])


def sfx_rug_warning():
    beeps = []
    for i in range(4):
        f = 780 if i % 2 == 0 else 620
        beeps.append(env(square(f, 0.28, 0.5) * 0.6, curve=3, attack=0.01))
        beeps.append(silence(0.08))
    return concat(*beeps)


def sfx_trade_complete():
    tick = env(noise(0.04), curve=9) * 0.5
    bell = mix(env(sine(2093, 0.8), curve=5), env(sine(2637, 0.7) * 0.6, curve=6))
    return concat(tick, bell)


def sfx_winner():
    seq = [392, 392, 392, 523.3]
    durs = [0.14, 0.14, 0.14, 0.7]
    parts = [env(mix(square(f, d, 0.45), tri(f / 2, d)) * 0.5, curve=2 if d > 0.5 else 5)
             for f, d in zip(seq, durs)]
    chord = mix(saw(523.3, 1.2), saw(659.3, 1.2), saw(784.0, 1.2)) * 0.2
    return concat(concat(*parts), env(chord, curve=2.5))


def sfx_emp_throw():
    return env(sweep(900, 250, 0.6) * 0.3 + noise(0.6) * 0.4, curve=3, attack=0.05)


def sfx_emp_blast():
    tt = t(1.1)
    fm = np.sin(2 * np.pi * 90 * tt + 8 * np.sin(2 * np.pi * 37 * tt))
    crackle = noise(1.1) * (rng.uniform(0, 1, len(tt)) > 0.93)
    return env(fm * 0.7 + crackle * 0.6, curve=4)


def sfx_office_upgrade():
    notes = [261.6, 293.7, 329.6, 349.2, 392.0, 440.0, 493.9, 523.3]
    run = concat(*[env(square(f, 0.11, 0.4) * 0.55, curve=7) for f in notes])
    sparkle = env(sweep(2000, 4000, 0.5) * 0.15, curve=4)
    return concat(run, sparkle)


def sfx_clip_saved():
    click1 = env(noise(0.03), curve=10) * 0.7
    whir = env(square(48, 0.25, 0.5) * 0.15 + noise(0.25) * 0.1, curve=2)
    click2 = env(noise(0.04), curve=10) * 0.5
    return concat(click1, whir, click2)


SFX = {
    "market_crash": sfx_market_crash,
    "market_moon": sfx_market_moon,
    "event_positive": sfx_event_positive,
    "event_negative": sfx_event_negative,
    "notification": sfx_notification,
    "phase_transition": sfx_phase_transition,
    "pump_group": sfx_pump_group,
    "rig_placed": sfx_rig_placed,
    "rig_upgrade": sfx_rig_upgrade,
    "rug_warning": sfx_rug_warning,
    "trade_complete": sfx_trade_complete,
    "winner": sfx_winner,
    "emp_throw": sfx_emp_throw,
    "emp_blast": sfx_emp_blast,
    "office_upgrade": sfx_office_upgrade,
    "clip_saved": sfx_clip_saved,
}


# ──────────────────────────────────────────────
#  Music — tiny chiptune tracker
# ──────────────────────────────────────────────

def midi(n):
    return 440.0 * 2 ** ((n - 69) / 12)


def render_track(bpm, bars, lanes):
    """
    lanes: list of (wave_fn, gain, pattern) where pattern is a list of
    (step16, midinote_or_None, len_steps). 16 steps per bar.
    """
    step = 60.0 / bpm / 4.0
    total = int(SR * step * 16 * bars)
    out = np.zeros(total + SR)  # headroom for tails

    for wave_fn, gain, pattern in lanes:
        for start, note, ln in pattern:
            dur = step * ln
            if note is None:  # drum hit: filtered noise
                sig = env(noise(min(dur, 0.09)), curve=9) * gain
            else:
                sig = env(wave_fn(midi(note), dur), curve=2.2, release=0.02) * gain
            i = int(start * step * SR)
            if i >= len(out):
                continue
            sig = sig[: len(out) - i]
            out[i:i + len(sig)] += sig

    return out[:total]  # cut tails at the loop point so it loops clean


def repeat_bars(pat, times, bar_steps=16):
    """Tile a one/two-bar pattern across the track."""
    out = []
    span = max(s + l for s, l, in [(p[0], p[2]) for p in pat]) if pat else bar_steps
    span = math.ceil(span / bar_steps) * bar_steps
    for r in range(times):
        out += [(s + r * span, n, l) for (s, n, l) in pat]
    return out


def music_create():
    """Create phase — 92 BPM lo-fi goblin workshop. A minor pentatonic."""
    bpm, bars = 92, 8
    bass_bar = [(0, 45, 3), (4, 45, 2), (8, 48, 3), (12, 43, 2)]
    lead_a = [(0, 69, 2), (3, 72, 1), (6, 76, 3), (12, 74, 2)]
    lead_b = [(0, 72, 2), (4, 69, 2), (8, 67, 3), (13, 64, 3)]
    lead = []
    for i in range(4):
        src = lead_a if i % 2 == 0 else lead_b
        lead += [(s + i * 32, n - (12 if i == 3 else 0), l) for s, n, l in src]
    hats = [(i, None, 1) for i in range(0, 16, 2)]
    kick = [(0, 33, 1), (10, 33, 1)]
    return render_track(bpm, bars, [
        (tri, 0.50, repeat_bars(bass_bar, bars)),
        (lambda f, d: square(f, d, 0.25), 0.16, lead),  # already spans all 8 bars
        (sine, 0.0, []),
        (tri, 0.35, repeat_bars(kick, bars)),
        (tri, 0.05, repeat_bars(hats, bars)),
    ])


def music_shill():
    """Shill phase — 118 BPM pump-it grindset. C major, relentless."""
    bpm, bars = 118, 8
    bass = [(0, 36, 1), (2, 48, 1), (4, 36, 1), (6, 48, 1),
            (8, 41, 1), (10, 53, 1), (12, 43, 1), (14, 55, 1)]
    lead_bar = [(0, 72, 1), (2, 76, 1), (4, 79, 2), (7, 76, 1),
                (8, 77, 1), (10, 74, 1), (12, 72, 2), (15, 67, 1)]
    clap = [(4, None, 1), (12, None, 1)]
    hats = [(i, None, 1) for i in range(1, 16, 2)]
    return render_track(bpm, bars, [
        (saw, 0.30, repeat_bars(bass, bars)),
        (lambda f, d: square(f, d, 0.5), 0.17, repeat_bars(lead_bar, bars)),
        (tri, 0.22, repeat_bars(clap, bars)),
        (tri, 0.06, repeat_bars(hats, bars)),
    ])


def music_chaos():
    """Chaos phase — 145 BPM sirens and margin calls. E phrygian."""
    bpm, bars = 145, 8
    bass = [(i, 40 if (i // 4) % 2 == 0 else 41, 1) for i in range(16)]
    siren_a = [(0, 76, 4), (4, 77, 4), (8, 76, 4), (12, 77, 4)]
    stabs = [(0, 64, 1), (3, 65, 1), (6, 64, 1), (10, 62, 1), (13, 60, 1)]
    hats = [(i, None, 1) for i in range(16)]
    kick = [(0, 33, 1), (4, 33, 1), (8, 33, 1), (12, 33, 1)]
    return render_track(bpm, bars, [
        (saw, 0.28, repeat_bars(bass, bars)),
        (lambda f, d: square(f, d, 0.15), 0.10, repeat_bars(siren_a, bars // 2, 32)),
        (lambda f, d: square(f, d, 0.5), 0.14, repeat_bars(stabs, bars)),
        (tri, 0.30, repeat_bars(kick, bars)),
        (tri, 0.05, repeat_bars(hats, bars)),
    ])


def music_results():
    """Results phase — 100 BPM ledger-is-final triumph. F Lydian-ish."""
    bpm, bars = 100, 8
    bass = [(0, 41, 4), (4, 45, 4), (8, 48, 4), (12, 43, 4)]
    chord_bar = []
    for beat, root in [(0, 65), (4, 69), (8, 72), (12, 67)]:
        for k, dn in enumerate((0, 4, 7)):
            chord_bar.append((beat + k, root + dn, 4 - k))
    lead = [(0, 77, 3), (4, 79, 3), (8, 81, 6), (16, 77, 2), (20, 74, 2),
            (24, 72, 6)]
    return render_track(bpm, bars, [
        (tri, 0.45, repeat_bars(bass, bars)),
        (lambda f, d: square(f, d, 0.3), 0.10, repeat_bars(chord_bar, bars)),
        (lambda f, d: square(f, d, 0.5), 0.15, repeat_bars(lead, bars // 2, 32)),
    ])


MUSIC = {
    "music_create": music_create,
    "music_shill": music_shill,
    "music_chaos": music_chaos,
    "music_results": music_results,
}


# ──────────────────────────────────────────────
#  Poster background art (text renders on top)
# ──────────────────────────────────────────────

W, H = 512, 384
PAPER = (245, 242, 224)


def poster_canvas():
    img = Image.new("RGB", (W, H), PAPER)
    return img, ImageDraw.Draw(img)


def grain(img, amount=6):
    px = np.array(img).astype(np.int16)
    px += rng.integers(-amount, amount + 1, px.shape, dtype=np.int16)
    return Image.fromarray(np.clip(px, 0, 255).astype(np.uint8))


def poster_sunburst(color):
    img, d = poster_canvas()
    cx, cy = W // 2, int(H * 0.62)
    for i in range(24):
        a0 = i * 15
        if i % 2 == 0:
            d.pieslice([cx - 700, cy - 700, cx + 700, cy + 700], a0, a0 + 15, fill=color)
    return img


def poster_diamond():
    img, d = poster_canvas()
    cx, cy, r = W // 2, H // 2, 110
    pts = [(cx, cy - r), (cx + r, cy), (cx, cy + r), (cx - r, cy)]
    d.polygon(pts, outline=(90, 120, 140), width=6)
    d.polygon([(cx - r, cy), (cx + r, cy), (cx, cy - r)], outline=(90, 120, 140), width=3)
    d.line([(cx - r // 2, cy - r // 2), (cx + r // 2, cy - r // 2)], fill=(90, 120, 140), width=3)
    return img


def poster_rug():
    img, d = poster_canvas()
    for y in range(0, H, 32):
        c = (196, 84, 62) if (y // 32) % 2 == 0 else (222, 168, 92)
        d.rectangle([40, y + 6, W - 40, y + 22], fill=c)
    d.polygon([(40, H - 90), (W - 40, H - 90), (W - 120, H - 20), (40, H - 40)],
              fill=PAPER, outline=(120, 90, 60), width=4)  # the rug, mid-pull
    return img


def poster_moon():
    img, d = poster_canvas()
    d.ellipse([W - 170, 30, W - 50, 150], fill=(226, 220, 200), outline=(160, 150, 130), width=4)
    x0, y0 = 60, H - 60
    pts = [(x0 + i * 24, y0 - (i * i) * 1.55) for i in range(16)]
    d.line(pts, fill=(120, 140, 120), width=5)
    return img


def poster_chart():
    img, d = poster_canvas()
    d.line([(50, H - 50), (W - 40, H - 50)], fill=(60, 60, 60), width=3)
    d.line([(50, H - 50), (50, 40)], fill=(60, 60, 60), width=3)
    xs = np.linspace(60, W - 60, 24)
    ys = H - 60 - np.linspace(0, H - 130, 24) - rng.uniform(-18, 18, 24)
    ys[-1] = 45  # number always go up
    d.line(list(zip(xs, ys)), fill=(84, 160, 96), width=6)
    return img


def poster_eye():
    img, d = poster_canvas()
    cx, cy = W // 2, H // 2
    d.polygon([(cx, cy - 120), (cx + 140, cy + 80), (cx - 140, cy + 80)],
              outline=(140, 120, 80), width=6)
    d.ellipse([cx - 45, cy - 20, cx + 45, cy + 50], outline=(140, 120, 80), width=5)
    d.ellipse([cx - 14, cy + 2, cx + 14, cy + 28], fill=(140, 120, 80))
    return img


def poster_bags():
    img, d = poster_canvas()
    for i, (bx, r) in enumerate([(110, 85), (270, 55), (390, 30)]):
        by = H - 70
        d.ellipse([bx - r, by - r, bx + r, by + r], outline=(150, 110, 70), width=5)
        d.polygon([(bx - r // 3, by - r), (bx + r // 3, by - r), (bx, by - r - 24)],
                  fill=(150, 110, 70))
        d.text((bx - 7, by - 12), "$", fill=(150, 110, 70))
    return img


def poster_waves():
    img, d = poster_canvas()
    for k in range(4):
        y = 90 + k * 70
        pts = [(x, y + math.sin(x / 36 + k) * 22 + k * 4) for x in range(30, W - 30, 6)]
        d.line(pts, fill=(110 + k * 20, 130, 150), width=4)
    return img


POSTER_ART = [
    lambda: poster_sunburst((228, 210, 170)),   # 0 DIAMOND HANDS
    lambda: poster_rug(),                       # 1 MOVE FAST AND RUG THINGS
    lambda: poster_moon(),                      # 2 TO THE MOON
    lambda: poster_chart(),                     # 3 PAST PERFORMANCE
    lambda: poster_eye(),                       # 4 IN GOBLINS WE TRUST
    lambda: poster_bags(),                      # 5 WHAT DOESN'T KILL YOUR BAG
    lambda: poster_waves(),                     # 6 BUYING THE DIP
    lambda: poster_sunburst((214, 190, 200)),   # 7 NGMI
]


# ──────────────────────────────────────────────
#  Main
# ──────────────────────────────────────────────

def main():
    os.makedirs(SOUNDS, exist_ok=True)
    os.makedirs(POSTERS, exist_ok=True)

    print("SFX:")
    for name, fn in SFX.items():
        write_wav(f"{name}.wav", fn())

    print("Music (loop lengths matter — MusicManager.cs hardcodes them):")
    for name, fn in MUSIC.items():
        write_wav(f"{name}.wav", fn())

    print("Posters:")
    for i, fn in enumerate(POSTER_ART):
        img = grain(fn())
        path = os.path.join(POSTERS, f"poster_{i}.png")
        img.save(path, optimize=True)
        print(f"  poster_{i}.png {img.size}")


if __name__ == "__main__":
    main()
